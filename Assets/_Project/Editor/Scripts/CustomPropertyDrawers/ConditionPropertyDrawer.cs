using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{
    [CustomPropertyDrawer(typeof(ConditionSelectorAttribute))]
    public class ConditionPropertyDrawer : PropertyDrawer
    {
        private const string NONE_LABEL = "Aucune";
        private static readonly Dictionary<string, Type[]> _typesByField = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.ManagedReference) {
                EditorGUI.LabelField(position, label.text, "[ConditionSelector] demande [SerializeReference]");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Type[] types = GetConcreteTypes(property.managedReferenceFieldTypename);
            int selectedIndex = Array.FindIndex(types, t => GetFullTypename(t) == property.managedReferenceFullTypename) + 1;

            GUIContent[] options = new GUIContent[types.Length + 1];
            options[0] = new GUIContent(NONE_LABEL);
            for (int i = 0; i < types.Length; i++) options[i + 1] = new GUIContent(types[i].Name);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float valueX = position.x + EditorGUIUtility.labelWidth + 2f;
            Rect popupRect = new(valueX, position.y, position.xMax - valueX, lineHeight);

            int newIndex = EditorGUI.Popup(popupRect, selectedIndex, options);
            if (newIndex != selectedIndex) {
                property.managedReferenceValue = newIndex == 0 ? null : Activator.CreateInstance(types[newIndex - 1]);
            }

            if (property.managedReferenceValue == null) {
                EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lineHeight), label);
            }
            else {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private static Type[] GetConcreteTypes(string fieldTypename) {
            if (_typesByField.TryGetValue(fieldTypename, out Type[] cached)) return cached;

            Type baseType = ResolveType(fieldTypename);
            Type[] types = baseType == null
                ? Array.Empty<Type>()
                : TypeCache.GetTypesDerivedFrom(baseType)
                    .Where(t => !t.IsAbstract && !t.IsGenericType && t.IsSerializable)
                    .Where(t => !typeof(UnityEngine.Object).IsAssignableFrom(t))
                    .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                    .OrderBy(t => t.Name)
                    .ToArray();

            _typesByField[fieldTypename] = types;
            return types;
        }

        private static Type ResolveType(string typename) {
            int split = typename.IndexOf(' ');
            if (split < 0) return null;
            return Type.GetType($"{typename.Substring(split + 1)}, {typename.Substring(0, split)}");
        }

        private static string GetFullTypename(Type type) => $"{type.Assembly.GetName().Name} {type.FullName}";
    }
}
