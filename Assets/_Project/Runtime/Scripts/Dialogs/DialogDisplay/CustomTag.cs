using System;
using System.Collections.Generic;
using System.Diagnostics;
using Codice.CM.SEIDInfo;
using NaughtyAttributes;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;
using UnityEngine.Android;
using Debug = UnityEngine.Debug;

namespace DShotgun.Dialogs
{
    [Serializable]
    public abstract class CustomTag<T>
    {
        private delegate void Parser(string value, out T toParse);

        private Dictionary<Type, Parser> parsers = new Dictionary<Type, Parser>()
        {
            { typeof(bool), BoolParser },
        };

        private static void BoolParser(string value, out bool toParse)
        {
            bool.TryParse(value, out toParse);
        }

        public abstract string Name { get; }
        protected TagId Id { get; private set; }
        public bool IsTag(string a_tag) => Id.IsTag(a_tag);
        public virtual bool TryGetValue(string a_tag, out T o_type) 
        {
            string value = Id.GetTagValueRaw(a_tag);
            o_type = default;
            
            if (value == String.Empty) return false;
            parsers[typeof(T)](a_tag, out o_type);

            if (type == typeof(string)) {
                o_type = (T)(object)value;
                return true;
            }
            else if (type == typeof(int)) {
                if (int.TryParse(value, out int result)) {
                    o_type = (T)(object)result;
                    return true;
                }
            }
            else if (type == typeof(float)) {
                if (float.TryParse(value, out float result)) {
                    o_type = (T)(object)result;
                    return true;
                }
            }
            else if (type == typeof(bool)) {
                if (bool.TryParse(value, out bool result)) {
                    o_type = (T)(object)result;
                    return true;
                }
            }
            else {
                Debug.LogError($"Cannot parse value, as it is {type} and not float, int, bool, string");
            }
            
            return false;
        }

        public virtual void UpdateTag(float a_delta, float a_time) {
            
        }

        protected CustomTag() {
            Id = new TagId(Name);
        }
    }

    [Serializable]
    public abstract class CustomMeshTag<T> : CustomTag<T>
    {
        protected List<Vector3[]> charactersVertices;
        public void AddCharacter(Vector3[] a_vertices) => charactersVertices.Add(a_vertices);
    }
    
    [Serializable]
    public struct TagId
    {
        [SerializeField, ReadOnly] private string _tagTextId;

        public TagId(string a_textId) {
            if (string.IsNullOrEmpty(a_textId) ||
                a_textId.Contains('/') ||
                a_textId.Contains('<') ||
                a_textId.Contains('>') ||
                a_textId.Contains('=') ||
                a_textId.Contains(' '))
            {
                _tagTextId = string.Empty;
                Debug.LogError($"Tag ID {a_textId} is invalid. May contain '/', '<', '>', '=',or ' '");
            }
            else
            {
                _tagTextId =  a_textId;
            }
        }

        public bool IsTag(string a_text) {
            if (string.IsNullOrEmpty(a_text)) return false;
            TrimTagSymbols(ref a_text);
            
            if (a_text.StartsWith(_tagTextId) &&
                (a_text.Length == _tagTextId.Length || a_text[_tagTextId.Length] == '='))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetTagValueRaw(string a_text) {
            if (string.IsNullOrEmpty(a_text)) return string.Empty;
            TrimTagSymbols(ref a_text);

            int i;
            for (i = 0; i < a_text.Length; i++) {
                if (a_text[i] == '=') {
                    i++;
                    break;
                }
            }
            
            if (i >= a_text.Length) return string.Empty;
            else return a_text.Substring(i);
        }
        
        private void TrimTagSymbols(ref string a_text) {
            a_text = a_text.Trim().Trim('<', '>').TrimStart('/');
        }
    }
}
