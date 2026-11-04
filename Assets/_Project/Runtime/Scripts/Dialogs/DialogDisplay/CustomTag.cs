using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DShotgun.Dialogs
{
    
    [Serializable]
    public abstract class CustomTag
    {
        protected abstract string Name { get; } // Name of the tag
        protected abstract CustomTagsType Type { get; } // Name of the tag
        public CustomTagsType GetType { get => Type; }
        
        
        protected TagId Id { get; private set; }
        public bool IsTag(string a_tag) => Id.IsTag(a_tag);

        public virtual void ChangeProfileValue(string a_tag, ref CharacterDialogProfile a_profile) {
            throw new NotImplementedException();
        }
        public virtual void ChangeInstructionValue(string a_tag, ref TextInstruction a_instruction) {
            throw new NotImplementedException();
        }
        
        public virtual bool TryGetValue<T>(string a_tag, out T o_type)
        {
            string value = Id.GetTagValueRaw(a_tag);
            o_type = default;
            
            if (value == String.Empty) return false;
            if (DSParser.TryGetParser<T>(out IDSParser<T> parser)) {
                if (!parser.TryParse(value, out o_type)) {
                    Debug.LogWarning($"'{Id.GetIdName}' Parsing error : value '{value}' couldn't be parsed as {typeof(T)}");
                    return false;
                }
                return true;
            }
            return false;
        }

        public virtual void UpdateTag(float a_delta, float a_time) {
            
        }

        protected CustomTag()
        {
            Id = new TagId(Name);
        }
    }

    [Serializable]
    public abstract class CustomMeshTag : CustomTag
    {
        protected override CustomTagsType Type { get => CustomTagsType.MeshTag; }
        
        protected List<Vector3[]> charactersVertices;
        public void AddCharacter(Vector3[] a_vertices) => charactersVertices.Add(a_vertices);
    }
    
    [Serializable]
    public struct TagId
    {
        [SerializeField, ReadOnly] private string _tagTextId;
        public string GetIdName { get => _tagTextId; }

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
            else {
                _tagTextId = a_textId.ToLower();
            }
        }

        public bool IsTag(string a_text) {
            if (string.IsNullOrEmpty(a_text)) return false;
            Debug.Log($"ISTAG BEFORE : {a_text}");
            TrimTagSymbols(ref a_text);
            Debug.Log($"ISTAG AFTER : {a_text}");
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

        public bool IsEndTag(string a_text) {
            return a_text[1] == '/';
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
            
            Debug.Log($"Raw value of '{a_text}' -> '{a_text.Substring(i)}'");
            
            if (i >= a_text.Length) return string.Empty;
            else return a_text.Substring(i);
        }
        
        private void TrimTagSymbols(ref string a_text) {
            a_text = a_text.Trim().Trim('<', '>').TrimStart('/');
        }
    }
    public enum CustomTagsType {
        None = -1,
        Other = 0,
        ProfileTag = 1,
        InstructionTag = 2,
        MeshTag = 3
    }
    
}
