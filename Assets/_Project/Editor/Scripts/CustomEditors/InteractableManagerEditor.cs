using System.Collections.Generic;
using System.Linq;
using DetectiveShotgun.Conditions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DetectiveShotgun.Editor
{
    [CustomEditor(typeof(InteractableManager))]
    public class InteractableManagerEditor : UnityEditor.Editor

    {
        private const string INTERACTION_CASES_FIELD = "_interactionCases";
        private const string CONDITION_FIELD = "_condition";
        private const string ACTION_FIELD = "_action";
        private const float VALUE_WIDTH = 18f;

        private SerializedProperty _interactionCases;
        private ReorderableList _list;

        private void OnEnable() {
            _interactionCases = serializedObject.FindProperty(INTERACTION_CASES_FIELD);
            _list = new ReorderableList(serializedObject, _interactionCases, true, true, true, true) {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawFlag,
                elementHeightCallback = GetElementHeight,
            };
        }

        private float GetElementHeight(int index)
        {
            SerializedProperty flag = _interactionCases.GetArrayElementAtIndex(index);
            SerializedProperty condition = flag.FindPropertyRelative(CONDITION_FIELD);
            SerializedProperty action = flag.FindPropertyRelative(ACTION_FIELD);

            float conditionHeight = EditorGUI.GetPropertyHeight(condition, GUIContent.none);
            float actionHeight = EditorGUI.GetPropertyHeight(action, GUIContent.none);
            float maxHeight = Mathf.Max(conditionHeight, actionHeight);

            return maxHeight + 8f;
        }
        
        private void DrawFlag(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty flag = _interactionCases.GetArrayElementAtIndex(index);
            SerializedProperty condition = flag.FindPropertyRelative(CONDITION_FIELD);
            SerializedProperty action = flag.FindPropertyRelative(ACTION_FIELD);

            rect.y += 2f;
            rect.height -= 4f;

            GUI.Box(rect, "");

            Rect boxRect = new(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f);

            float conditionHeight = EditorGUI.GetPropertyHeight(condition, GUIContent.none);
            float actionHeight = EditorGUI.GetPropertyHeight(action, GUIContent.none);
            float maxHeight = Mathf.Max(conditionHeight, actionHeight);

            float conditionWidth = boxRect.width * 0.6f - 4f;
            float actionWidth = boxRect.width * 0.4f - 4f;

            Rect conditionRect = new(boxRect.x, boxRect.y, conditionWidth, conditionHeight);
            Rect actionRect = new(boxRect.xMax - actionWidth, boxRect.y, actionWidth, actionHeight);

            EditorGUI.PropertyField(conditionRect, condition, GUIContent.none);
            EditorGUI.PropertyField(actionRect, action, GUIContent.none);
        }
        public override void OnInspectorGUI() {
            serializedObject.Update();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Flags");
            Rect valueRect = new(rect.xMax - 80f, rect.y, 80f, rect.height);
            EditorGUI.LabelField(valueRect, "Valeur initiale", EditorStyles.miniLabel);
        }
    }
}
