using System;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{

    [CustomPropertyDrawer(typeof(BlackboardData))]
    public class BlackboardDataDrawer : PropertyDrawer
    {
        private const string NONE_LABEL = "Aucun";
        private const float BUTTON_WIDTH = 44f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            BlackboardData[] allData = BlackboardEditorUtility.FindAllData();

            GUIContent[] options = new GUIContent[allData.Length + 1];
            options[0] = new GUIContent(NONE_LABEL);
            for (int i = 0; i < allData.Length; i++) {
                options[i + 1] = new GUIContent(allData[i].name, AssetDatabase.GetAssetPath(allData[i]));
            }

            EditorGUI.BeginProperty(position, label, property);

            int selectedIndex = Array.IndexOf(allData, property.objectReferenceValue as BlackboardData) + 1;
            Rect popupRect = new(position.x, position.y, position.width - BUTTON_WIDTH - 2f, position.height);
            Rect buttonRect = new(position.xMax - BUTTON_WIDTH, position.y, BUTTON_WIDTH, position.height);

            int newIndex = EditorGUI.Popup(popupRect, label, selectedIndex, options);
            if (newIndex != selectedIndex) {
                property.objectReferenceValue = newIndex == 0 ? null : allData[newIndex - 1];
            }

            using (new EditorGUI.DisabledScope(property.objectReferenceValue == null)) {
                if (GUI.Button(buttonRect, new GUIContent("Voir", "Sélectionner l'asset dans le Project"))) {
                    EditorGUIUtility.PingObject(property.objectReferenceValue);
                    Selection.activeObject = property.objectReferenceValue;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
