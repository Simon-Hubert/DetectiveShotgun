using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun.Dialogs
{
    [Serializable]
    public struct NamedSprite
    {
        public NamedSprite(string name, Sprite sprite) {
            Name = name;
            Sprite = sprite;
        }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
    
    [Serializable]
    public struct CharacterData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, Expandable] public SOCharacterDialogProfile DialogProfile { get; private set; }
        
        [SerializeField] private List<NamedSprite> _spriteList;
        
        public CharacterData(string name, List<NamedSprite> spriteList, SOCharacterDialogProfile dialogProfile) {
            Name = name;
            _spriteList = spriteList;
            DialogProfile = dialogProfile;
        }
        
        public IReadOnlyList<NamedSprite> SpriteList => _spriteList;
    }
    
    [CreateAssetMenu(fileName = "CharactersDatabase", menuName = "Databases/CharactersDatabase")]
    public class CharactersDatabase : ScriptableObject
    {
        [SerializeField] private List<CharacterData> _characters = new List<CharacterData>();
        public IReadOnlyList<CharacterData> Characters => _characters;
        
        public CharacterData GetCharacter(string name) {
            foreach (CharacterData characterData in _characters.Where(characterData => characterData.Name == name)) {
                return characterData;
            }
            return new CharacterData("", null, CreateInstance<SOCharacterDialogProfile>());
        }
    }
}
