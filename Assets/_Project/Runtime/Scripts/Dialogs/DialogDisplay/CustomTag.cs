using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun.Dialogs
{
    [Serializable]
    public abstract class CustomTag
    {
        protected abstract string Name { get; }
        public TagId Id { get; private set; }

        public CustomTag() {
            Id = new TagId(Name);
        }
    }

    public abstract class CustomMeshTag : CustomTag
    {
        protected List<Vector3[]> charactersVertices;
        public void AddCharacter(Vector3[] a_vertices) => charactersVertices.Add(a_vertices);

        public abstract void UpdateTag(float a_delta, float a_time);
    }
    
    public struct TagId
    {
        private string _tagTextId;

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
