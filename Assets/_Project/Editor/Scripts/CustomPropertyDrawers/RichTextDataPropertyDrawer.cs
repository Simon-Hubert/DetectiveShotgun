using System.Collections.Generic;
using System.Reflection;
using DShotgun.RichText;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{
    // Front volontairement simple : le texte balisé est édité tel quel, les boutons insèrent des balises,
    // et toute la logique (vérification, aperçu, erreurs) vient de RichTextCompiler.
    [CustomPropertyDrawer(typeof(RichTextData))]
    public class RichTextDataPropertyDrawer : PropertyDrawer
    {
        private const float TOOLBAR_HEIGHT = 21f;   // hauteur du style EditorStyles.toolbar
        private const float TAB_WIDTH = 64f;
        private const float STYLE_BUTTON_WIDTH = 26f;
        private const float COLOR_BUTTON_WIDTH = 60f;
        private const int MIN_TEXT_LINES = 4;
        private const int MAX_DIAGNOSTICS = 4;
        private const float SPACING = 2f;
        private const float HELPBOX_ICON_WIDTH = 40f;
        private const float INSPECTOR_MARGIN = 40f;   // largeur estimée tant que la vraie n'est pas connue
        private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

        // TextEditor interne d'EditorGUI.TextArea : GetStateObject ne le renvoie pas.
        private static readonly FieldInfo ACTIVE_EDITOR_FIELD =
            typeof(EditorGUI).GetField("activeEditor", BindingFlags.NonPublic | BindingFlags.Static);

        // Un PropertyDrawer est recréé et partagé par Unity : l'état de chaque champ vit ici.
        private class FieldState
        {
            public bool IsPreview;
            public float Width;                 // GetPropertyHeight ne reçoit pas la largeur

            // Dernière sélection connue : la zone perd le focus au clic sur un bouton
            public bool HasSelection;
            public int SelectionStart;
            public int SelectionEnd;

            // Sélection à remettre dans la zone après une insertion de balises
            public bool HasPendingSelection;
            public int PendingStart;
            public int PendingEnd;
            public bool RequestFocus;

            public string CompiledSource;
            public CompileResult Compiled;
        }
        private static readonly Dictionary<string, FieldState> _stateByField = new();

        private static GUIStyle _boldButtonStyle;
        private static GUIStyle _italicButtonStyle;
        private static GUIStyle _textAreaStyle;
        private static GUIStyle _previewStyle;
        private static GUIStyle _placeholderStyle;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitStyles();
            FieldState state = GetState(property);
            float width = state.Width > 1f ? state.Width : EditorGUIUtility.currentViewWidth - INSPECTOR_MARGIN;
            string source = property.FindPropertyRelative("_text").stringValue;
            CompileResult compiled = GetCompiled(state, source);

            return LINE_HEIGHT + TOOLBAR_HEIGHT
                + GetContentHeight(state, source, compiled, width)
                + GetDiagnosticsHeight(compiled, width);
        }

        // Le conteneur IMGUI par défaut de l'inspecteur ne suit pas les changements de hauteur : celui-ci si.
        public override UnityEngine.UIElements.VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedObject serializedObject = property.serializedObject;
            string propertyPath = property.propertyPath;
            GUIContent label = new GUIContent(property.displayName, property.tooltip);

            UnityEngine.UIElements.IMGUIContainer container = null;
            container = new UnityEngine.UIElements.IMGUIContainer(() =>
            {
                if (serializedObject.targetObject == null) return;

                serializedObject.UpdateIfRequiredOrScript(); // relis l'obj que si il a vrmt changé
                SerializedProperty current = serializedObject.FindProperty(propertyPath);
                if (current == null) return;

                float width = container.contentRect.width;
                if (width > 1f) GetState(current).Width = width;

                Rect rect = GUILayoutUtility.GetRect(0f, GetPropertyHeight(current, label), GUILayout.ExpandWidth(true));
                OnGUI(rect, current, label);

                serializedObject.ApplyModifiedProperties();
            });
            return container;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitStyles();

            SerializedProperty text = property.FindPropertyRelative("_text");
            MigrateLegacyStyles(property, text);

            string controlName = "RichText_" + property.propertyPath;
            FieldState state = GetState(property);

            if (Event.current.type == EventType.Repaint && position.width > 1f)   // largeur fausse hors Repaint
            {
                state.Width = position.width;
            }

            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            EditorGUI.LabelField(new Rect(position.x, y, position.width, LINE_HEIGHT), label);
            y += LINE_HEIGHT;

            DrawToolbar(new Rect(position.x, y, position.width, TOOLBAR_HEIGHT), property, text, state, controlName);
            y += TOOLBAR_HEIGHT;

            CompileResult compiled = GetCompiled(state, text.stringValue);
            float contentHeight = GetContentHeight(state, text.stringValue, compiled, position.width);
            Rect contentRect = new Rect(position.x, y, position.width, contentHeight);

            if (state.IsPreview) DrawPreview(contentRect, compiled);
            else DrawTextArea(contentRect, text, state, controlName);
            y += contentHeight;

            DrawDiagnostics(position.x, y, position.width, compiled);

            EditorGUI.EndProperty();
        }

        private static void DrawToolbar(Rect rect, SerializedProperty property, SerializedProperty text, FieldState state, string controlName)
        {
            if (Event.current.type == EventType.Repaint) EditorStyles.toolbar.Draw(rect, false, false, false, false);

            bool writeTab = GUI.Toggle(new Rect(rect.x, rect.y, TAB_WIDTH, rect.height), !state.IsPreview, "Écrire", EditorStyles.toolbarButton);
            bool previewTab = GUI.Toggle(new Rect(rect.x + TAB_WIDTH, rect.y, TAB_WIDTH, rect.height), state.IsPreview, "Aperçu", EditorStyles.toolbarButton);

            if (state.IsPreview && writeTab) state.IsPreview = false;
            else if (!state.IsPreview && previewTab)
            {
                state.IsPreview = true;
                GUI.FocusControl(null);   // pas de saisie clavier en aperçu
            }

            using (new EditorGUI.DisabledScope(state.IsPreview))
            {
                float x = rect.xMax - (STYLE_BUTTON_WIDTH * 2f + COLOR_BUTTON_WIDTH);

                if (GUI.Button(new Rect(x, rect.y, STYLE_BUTTON_WIDTH, rect.height), new GUIContent("G", "Gras : <b>…</b>"), _boldButtonStyle))
                    WrapSelection(text, state, controlName, "<b>", "</b>");
                x += STYLE_BUTTON_WIDTH;

                if (GUI.Button(new Rect(x, rect.y, STYLE_BUTTON_WIDTH, rect.height), new GUIContent("I", "Italique : <i>…</i>"), _italicButtonStyle))
                    WrapSelection(text, state, controlName, "<i>", "</i>");
                x += STYLE_BUTTON_WIDTH;

                Rect colorButtonRect = new Rect(x, rect.y, COLOR_BUTTON_WIDTH, rect.height);
                if (GUI.Button(colorButtonRect, new GUIContent("Couleur", "<color=#RRGGBBAA>…</color>"), EditorStyles.toolbarButton))
                    OpenColorWindow(colorButtonRect, property, state);
            }
        }

        private static void DrawTextArea(Rect rect, SerializedProperty text, FieldState state, string controlName)
        {
            if (state.RequestFocus)
            {
                GUI.FocusControl(controlName);
                state.RequestFocus = false;
            }

            GUI.SetNextControlName(controlName);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUI.TextArea(rect, text.stringValue, _textAreaStyle);
            if (EditorGUI.EndChangeCheck()) text.stringValue = newText;   // sinon Apply à chaque événement → lag

            if (Event.current.type != EventType.Repaint || GUI.GetNameOfFocusedControl() != controlName) return;
            if (!(ACTIVE_EDITOR_FIELD?.GetValue(null) is TextEditor editor)) return;

            // L'éditeur garde sa propre copie du texte : sans ça, une insertion par bouton serait écrasée.
            if (editor.text != text.stringValue) editor.text = text.stringValue;

            if (state.HasPendingSelection)
            {
                editor.selectIndex = Mathf.Clamp(state.PendingStart, 0, editor.text.Length);
                editor.cursorIndex = Mathf.Clamp(state.PendingEnd, 0, editor.text.Length);
                state.HasPendingSelection = false;
            }

            state.HasSelection = true;
            state.SelectionStart = Mathf.Min(editor.cursorIndex, editor.selectIndex);
            state.SelectionEnd = Mathf.Max(editor.cursorIndex, editor.selectIndex);
        }

        private static void WrapSelection(SerializedProperty text, FieldState state, string controlName, string open, string close)
        {
            // Jamais cliqué dans la zone : les balises vont à la fin du texte
            int start = state.HasSelection ? state.SelectionStart : text.stringValue.Length;
            int end = state.HasSelection ? state.SelectionEnd : text.stringValue.Length;

            text.stringValue = RichTextTagInserter.Wrap(text.stringValue, start, end, open, close, out int newStart, out int newEnd);
            SelectAfterEdit(state, newStart, newEnd);
            GUI.FocusControl(controlName);
        }

        private static void SelectAfterEdit(FieldState state, int start, int end)
        {
            state.SelectionStart = start;
            state.SelectionEnd = end;
            state.PendingStart = start;
            state.PendingEnd = end;
            state.HasPendingSelection = true;
            state.RequestFocus = true;
        }

        // La couleur arrive plus tard, quand `property` n'est plus valide : on repasse par un SerializedObject neuf
        private static void OpenColorWindow(Rect buttonRect, SerializedProperty property, FieldState state)
        {
            Object[] targets = property.serializedObject.targetObjects;
            string textPath = property.propertyPath + "._text";
            string currentText = property.FindPropertyRelative("_text").stringValue;
            int start = state.HasSelection ? state.SelectionStart : currentText.Length;
            int end = state.HasSelection ? state.SelectionEnd : currentText.Length;

            ColorPaletteWindow.Open(buttonRect, FindPalette(), Color.white, color =>
            {
                if (targets == null || targets.Length == 0 || targets[0] == null) return;

                using SerializedObject serializedObject = new SerializedObject(targets);
                serializedObject.Update();
                SerializedProperty text = serializedObject.FindProperty(textPath);
                if (text == null) return;

                string open = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
                text.stringValue = RichTextTagInserter.Wrap(text.stringValue, start, end, open, "</color>", out int newStart, out int newEnd);
                serializedObject.ApplyModifiedProperties();

                SelectAfterEdit(state, newStart, newEnd);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            });
        }

        private static void DrawPreview(Rect rect, CompileResult compiled)
        {
            if (string.IsNullOrEmpty(compiled.Output)) GUI.Label(rect, "Rien à prévisualiser.", _placeholderStyle);
            else GUI.Label(rect, compiled.Output, _previewStyle);
        }

        private static void DrawDiagnostics(float x, float y, float width, CompileResult compiled)
        {
            int shown = Mathf.Min(compiled.Diagnostics.Count, MAX_DIAGNOSTICS);
            for (int i = 0; i < shown; i++)
            {
                Diagnostic diagnostic = compiled.Diagnostics[i];
                string message = FormatDiagnostic(diagnostic);
                float height = GetHelpBoxHeight(message, width);
                y += SPACING;
                MessageType type = diagnostic.Level == DiagnosticLevel.Error ? MessageType.Error : MessageType.Warning;
                EditorGUI.HelpBox(new Rect(x, y, width, height), message, type);
                y += height;
            }

            int hidden = compiled.Diagnostics.Count - shown;
            if (hidden > 0)
            {
                EditorGUI.LabelField(new Rect(x, y + SPACING, width, LINE_HEIGHT), $"… et {hidden} autre(s)", EditorStyles.miniLabel);
            }
        }

        private static float GetDiagnosticsHeight(CompileResult compiled, float width)
        {
            float height = 0f;
            int shown = Mathf.Min(compiled.Diagnostics.Count, MAX_DIAGNOSTICS);
            for (int i = 0; i < shown; i++) height += SPACING + GetHelpBoxHeight(FormatDiagnostic(compiled.Diagnostics[i]), width);
            if (compiled.Diagnostics.Count > shown) height += SPACING + LINE_HEIGHT;
            return height;
        }

        private static string FormatDiagnostic(Diagnostic diagnostic) =>
            $"Ligne {diagnostic.Line}, colonne {diagnostic.Column} : {diagnostic.Message}";

        private static float GetHelpBoxHeight(string message, float width) =>
            Mathf.Max(EditorStyles.helpBox.CalcHeight(new GUIContent(message), width - HELPBOX_ICON_WIDTH), LINE_HEIGHT * 2f);

        private static float GetContentHeight(FieldState state, string source, CompileResult compiled, float width)
        {
            float textHeight;
            if (state.IsPreview)
            {
                textHeight = _previewStyle.CalcHeight(new GUIContent(compiled.Output), width);
            }
            else
            {
                if (source.EndsWith("\n")) source += " ";   // CalcHeight ignore une ligne vide finale
                textHeight = _textAreaStyle.CalcHeight(new GUIContent(source), width);
            }
            return Mathf.Max(LINE_HEIGHT * MIN_TEXT_LINES, textHeight);
        }

        // Recompile seulement si le texte a changé
        private static CompileResult GetCompiled(FieldState state, string source)
        {
            if (state.Compiled == null || state.CompiledSource != source)
            {
                state.Compiled = RichTextCompiler.Compile(source, RichTextTarget.Preview);
                state.CompiledSource = source;
            }
            return state.Compiled;
        }

        // Ancien format (texte brut + liste _styles) → texte balisé, une seule fois par champ
        private static void MigrateLegacyStyles(SerializedProperty property, SerializedProperty text)
        {
            SerializedProperty legacyStyles = property.FindPropertyRelative("_styles");
            if (legacyStyles == null || legacyStyles.arraySize == 0) return;

            text.stringValue = RichTextFormatter.GenerateTaggedText(property.boxedValue as RichTextData);
            legacyStyles.arraySize = 0;
            Debug.Log($"RichText : « {property.propertyPath} » de {property.serializedObject.targetObject.name} converti au format balisé.",
                property.serializedObject.targetObject);
        }

        private static string GetFieldKey(SerializedProperty property) =>
            property.serializedObject.targetObject.GetInstanceID() + "/" + property.propertyPath;

        private static FieldState GetState(SerializedProperty property)
        {
            string fieldKey = GetFieldKey(property);
            if (!_stateByField.TryGetValue(fieldKey, out FieldState state))
            {
                state = new FieldState();
                _stateByField[fieldKey] = state;
            }
            return state;
        }

        private static ColorPalette FindPalette()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ColorPalette)}");
            return guids.Length == 0 ? null : AssetDatabase.LoadAssetAtPath<ColorPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        // EditorStyles n'est pas prêt à l'initialisation statique
        private static void InitStyles()
        {
            if (_previewStyle != null) return;

            _boldButtonStyle = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };
            _italicButtonStyle = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Italic };
            _textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
            _previewStyle = new GUIStyle(EditorStyles.textArea) { richText = true, wordWrap = true };
            _placeholderStyle = new GUIStyle(_previewStyle) { fontStyle = FontStyle.Italic };
            _placeholderStyle.normal.textColor = Color.gray;
        }
    }
}
