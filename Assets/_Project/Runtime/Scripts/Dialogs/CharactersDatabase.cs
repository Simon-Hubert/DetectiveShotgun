using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DShotgun.Dialogs
{
    [Serializable]
    public struct NamedSprites
    {
        public NamedSprites(string name, Sprite sprite) {
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
        [SerializeField] private List<NamedSprites> _spriteList;
        
        public CharacterData(string name, List<NamedSprites> spriteList) {
            Name = name;
            _spriteList = spriteList;
        }
        public IReadOnlyList<NamedSprites> SpriteList => _spriteList;
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
            return new CharacterData("MissingCharaceter", null);
        }
    }
}
