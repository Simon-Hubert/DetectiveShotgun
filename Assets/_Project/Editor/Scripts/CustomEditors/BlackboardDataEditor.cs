using System.Collections.Generic;
using System.Linq;
using DetectiveShotgun.Conditions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DetectiveShotgun.Editor
{
    [CustomEditor(typeof(BlackboardData))]
    public class BlackboardDataEditor : UnityEditor.Editor
    {
        private const string FLAGS_FIELD = "_flags";
        private const string NAME_FIELD = "_name";
        private const string VALUE_FIELD = "_initializedValue";
        private const string DEFAULT_NAME_PREFIX = "Element_";
        private const float VALUE_WIDTH = 18f;

        private SerializedProperty _flags;
        private ReorderableList _list;

        private void OnEnable() {
            _flags = serializedObject.FindProperty(FLAGS_FIELD);
            _list = new ReorderableList(serializedObject, _flags, true, true, true, true) {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawFlag,
                onAddCallback = AddFlag,
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            _list.DoLayoutList();
            DrawWarnings();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Flags");
            Rect valueRect = new(rect.xMax - 80f, rect.y, 80f, rect.height);
            EditorGUI.LabelField(valueRect, "Valeur initiale", EditorStyles.miniLabel);
        }

        private void DrawFlag(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty flag = _flags.GetArrayElementAtIndex(index);
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            Rect nameRect = new(rect.x, rect.y, rect.width - VALUE_WIDTH - 8f, rect.height);
            Rect valueRect = new(rect.xMax - VALUE_WIDTH, rect.y, VALUE_WIDTH, rect.height);

            EditorGUI.PropertyField(nameRect, flag.FindPropertyRelative(NAME_FIELD), GUIContent.none);
            EditorGUI.PropertyField(valueRect, flag.FindPropertyRelative(VALUE_FIELD), GUIContent.none);
        }

        private void AddFlag(ReorderableList list) {
            int index = _flags.arraySize;
            _flags.InsertArrayElementAtIndex(index);
            SerializedProperty flag = _flags.GetArrayElementAtIndex(index);
            flag.FindPropertyRelative(NAME_FIELD).stringValue = GetUniqueName(index);
            flag.FindPropertyRelative(VALUE_FIELD).boolValue = false;
            list.index = index;
        }

        private string GetUniqueName(int index) {
            HashSet<string> usedNames = new();
            for (int i = 0; i < _flags.arraySize; i++) {
                if (i == index) continue;
                usedNames.Add(_flags.GetArrayElementAtIndex(i).FindPropertyRelative(NAME_FIELD).stringValue);
            }

            int suffix = index;
            while (usedNames.Contains(DEFAULT_NAME_PREFIX + suffix)) suffix++;
            return DEFAULT_NAME_PREFIX + suffix;
        }

        private void DrawWarnings() {
            List<string> names = new();
            for (int i = 0; i < _flags.arraySize; i++) {
                names.Add(_flags.GetArrayElementAtIndex(i).FindPropertyRelative(NAME_FIELD).stringValue);
            }

            if (names.Any(string.IsNullOrWhiteSpace)) {
                EditorGUILayout.HelpBox("Un flag n'a pas de nom : il sera ignoré.", MessageType.Warning);
            }

            string[] duplicates = names.Where(n => !string.IsNullOrWhiteSpace(n))
                .GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
            if (duplicates.Length > 0) {
                EditorGUILayout.HelpBox($"Noms en double : {string.Join(", ", duplicates)}", MessageType.Error);
            }
        }
    }
}
