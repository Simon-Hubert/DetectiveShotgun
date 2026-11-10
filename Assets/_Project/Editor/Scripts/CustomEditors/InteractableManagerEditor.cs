using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DShotgun.Editor
{
    [CustomEditor(typeof(InteractableManager))]
    public class InteractableManagerEditor : UnityEditor.Editor

    {
        private const string INTERACTION_CASES_FIELD = "_interactionCases";
        private const string AIM_CASES_FIELD = "_aimCases";
        private const string CONDITION_FIELD = "_condition";
        private const string ACTION_FIELD = "_action";
        private const float VALUE_WIDTH = 18f;

        private SerializedProperty _interactionCases;
        private ReorderableList _list;
        private SerializedProperty _aimCases;
        private ReorderableList _aimList;
        private SerializedProperty _priority;
        private SerializedProperty _onInteracted;
        private SerializedProperty _onAimed;

        private void OnEnable() {
            _interactionCases = serializedObject.FindProperty(INTERACTION_CASES_FIELD);
            _list = new ReorderableList(serializedObject, _interactionCases, true, true, true, true) {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawFlag,
                elementHeightCallback = GetElementHeight,
            };
            
            _aimCases = serializedObject.FindProperty(AIM_CASES_FIELD);
            _aimList = new ReorderableList(serializedObject, _aimCases, true, true, true, true) {
                drawHeaderCallback = DrawAimHeader,
                drawElementCallback = DrawAimFlag,
                elementHeightCallback = GetAimElementHeight,
            };

            _priority = serializedObject.FindProperty("_priority");
            _onInteracted = serializedObject.FindProperty("_onInteracted");
            _onAimed = serializedObject.FindProperty("_onAimed");
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
            EditorGUILayout.PropertyField(_priority);
            _list.DoLayoutList();
            EditorGUILayout.PropertyField(_onInteracted);
            _aimList.DoLayoutList();
            EditorGUILayout.PropertyField(_onAimed);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Flags");
            Rect valueRect = new(rect.xMax - 80f, rect.y, 80f, rect.height);
            EditorGUI.LabelField(valueRect, "Valeur initiale", EditorStyles.miniLabel);
        }

        private float GetAimElementHeight(int index)
        {
            SerializedProperty flag = _aimCases.GetArrayElementAtIndex(index);
            SerializedProperty condition = flag.FindPropertyRelative(CONDITION_FIELD);
            SerializedProperty action = flag.FindPropertyRelative(ACTION_FIELD);

            float conditionHeight = EditorGUI.GetPropertyHeight(condition, GUIContent.none);
            float actionHeight = EditorGUI.GetPropertyHeight(action, GUIContent.none);
            float maxHeight = Mathf.Max(conditionHeight, actionHeight);

            return maxHeight + 8f;
        }

        private void DrawAimFlag(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty flag = _aimCases.GetArrayElementAtIndex(index);
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

        private void DrawAimHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Aim Flags");
            Rect valueRect = new(rect.xMax - 80f, rect.y, 80f, rect.height);
            EditorGUI.LabelField(valueRect, "Valeur initiale", EditorStyles.miniLabel);
        }
    }
}
