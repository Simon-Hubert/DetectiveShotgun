using System.Collections.Generic;
using System.Linq;
using DetectiveShotgun.Conditions;
using UnityEditor;
using UnityEngine;

namespace DetectiveShotgun.Editor
{
    [CustomPropertyDrawer(typeof(FlagReference))]
    public class FlagReferenceDrawer : PropertyDrawer
    {
        private const string NAME_FIELD = "_flagName";
        private const string NONE_LABEL = "Aucun";
        private const string MISSING_SUFFIX = " (introuvable)";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty nameProperty = property.FindPropertyRelative(NAME_FIELD);
            BlackboardData data = BlackboardEditorUtility.FindData(property.serializedObject);

            EditorGUI.BeginProperty(position, label, property);

            if (data == null) {
                GUIContent fallbackLabel = new(label.text + " (aucun Blackboard)",
                    "Renseigne le champ Blackboard de l'objet, et le BlackboardData de ce Blackboard, pour choisir dans une liste.");
                EditorGUI.PropertyField(position, nameProperty, fallbackLabel);
                EditorGUI.EndProperty();
                return;
            }

            List<string> names = data.Flags
                .Select(flag => flag.Name)
                .Where(flagName => !string.IsNullOrEmpty(flagName))
                .Distinct()
                .ToList();

            string current = nameProperty.stringValue;
            bool isMissing = !string.IsNullOrEmpty(current) && !names.Contains(current);

            List<GUIContent> options = new() { new GUIContent(NONE_LABEL) };
            options.AddRange(names.Select(flagName => new GUIContent(flagName)));
            if (isMissing) options.Add(new GUIContent(current + MISSING_SUFFIX));

            int selectedIndex = string.IsNullOrEmpty(current) ? 0
                : isMissing ? options.Count - 1
                : names.IndexOf(current) + 1;

            int newIndex = EditorGUI.Popup(position, label, selectedIndex, options.ToArray());
            if (newIndex != selectedIndex) {
                if (newIndex == 0) nameProperty.stringValue = string.Empty;
                else if (newIndex <= names.Count) nameProperty.stringValue = names[newIndex - 1];
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
