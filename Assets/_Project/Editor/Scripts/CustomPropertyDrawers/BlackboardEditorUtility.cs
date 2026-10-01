using System.Linq;
using UnityEditor;

namespace DShotgun.Editor
{
    internal static class BlackboardEditorUtility
    {
        private const string BLACKBOARD_FIELD = "_blackboard";
        private const string BLACKBOARD_DATA_FIELD = "_blackboardData";
        
        public static BlackboardData FindData(SerializedObject serializedObject) {
            SerializedProperty blackboardProperty = serializedObject.FindProperty(BLACKBOARD_FIELD);
            if (blackboardProperty == null || blackboardProperty.propertyType != SerializedPropertyType.ObjectReference) return null;

            switch (blackboardProperty.objectReferenceValue) {
                case BlackboardData data: return data;
                case Blackboard blackboard: return GetData(blackboard);
                default: return null;
            }
        }

        public static BlackboardData GetData(Blackboard blackboard) {
            using SerializedObject serializedBlackboard = new(blackboard);
            return serializedBlackboard.FindProperty(BLACKBOARD_DATA_FIELD)?.objectReferenceValue as BlackboardData;
        }

        public static BlackboardData[] FindAllData() {
            return AssetDatabase.FindAssets($"t:{nameof(BlackboardData)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<BlackboardData>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(data => data != null)
                .OrderBy(data => data.name)
                .ToArray();
        }
    }
}
