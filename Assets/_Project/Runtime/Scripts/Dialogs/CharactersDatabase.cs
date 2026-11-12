using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun.Dialogs
{
    [Serializable]
    public struct NamedSprite
    {
        public NamedSprite(string name, Sprite sprite, int id = 0) {
            Id = id;
            Name = name;
            Sprite = sprite;
        }
        [field: SerializeField, HideInInspector] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }

    [Serializable]
    public struct CharacterData
    {
        [field: SerializeField, HideInInspector] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, Expandable] public SOCharacterDialogProfile DialogProfile { get; private set; }

        [SerializeField] private List<NamedSprite> _spriteList;

        public CharacterData(string name, List<NamedSprite> spriteList, SOCharacterDialogProfile dialogProfile, int id = 0) {
            Id = id;
            Name = name;
            _spriteList = spriteList;
            DialogProfile = dialogProfile;
        }

        public IReadOnlyList<NamedSprite> SpriteList => _spriteList;
        
        public Sprite GetSprite(int spriteId) {
            Sprite first = null;
            foreach (NamedSprite named in _spriteList) {
                if (named.Id == spriteId && spriteId > 0) return named.Sprite;
                if (first == null) first = named.Sprite;
            }
            return first;
        }
    }

    [CreateAssetMenu(fileName = "CharactersDatabase", menuName = "Databases/CharactersDatabase")]
    public class CharactersDatabase : ScriptableObject
    {
        public const string RESOURCE_PATH = "Characters";

        [SerializeField] private List<CharacterData> _characters = new List<CharacterData>();
        [SerializeField, HideInInspector] private int _nextId = 1;   // ne redescend jamais : l'ID d'un personnage supprimé n'est pas réutilisé
        [SerializeField, HideInInspector] private int _nextSpriteId = 1;
        public IReadOnlyList<CharacterData> Characters => _characters;

        public static CharactersDatabase Load() => Resources.Load<CharactersDatabase>(RESOURCE_PATH);

        public bool TryGetCharacter(int id, out CharacterData character) {
            foreach (CharacterData candidate in _characters) {
                if (id <= 0 || candidate.Id != id) continue;
                character = candidate;
                return true;
            }
            character = default;
            return false;
        }

#if UNITY_EDITOR
        // Les IDs doivent être écrits dans l'asset : sinon ils sont recalculés d'après l'ordre de la liste et ça me fait peur
        [UnityEditor.InitializeOnLoadMethod]
        private static void SaveIdsOnLoad() {
            UnityEditor.EditorApplication.delayCall += () => {
                CharactersDatabase database = Load();
                if (database == null || !database.EnsureIds()) return;
                UnityEditor.EditorUtility.SetDirty(database);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(database);
            };
        }

        private bool EnsureIds() {
            foreach (CharacterData character in _characters) {
                if (character.Id >= _nextId) _nextId = character.Id + 1;
                foreach (NamedSprite sprite in character.SpriteList) {
                    if (sprite.Id >= _nextSpriteId) _nextSpriteId = sprite.Id + 1;
                }
            }

            bool changed = false;
            HashSet<int> usedCharacters = new HashSet<int>();
            HashSet<int> usedSprites = new HashSet<int>();
            for (int i = 0; i < _characters.Count; i++) {
                CharacterData character = _characters[i];
                List<NamedSprite> sprites = new List<NamedSprite>(character.SpriteList);
                bool characterChanged = false;

                for (int j = 0; j < sprites.Count; j++) {
                    if (sprites[j].Id > 0 && usedSprites.Add(sprites[j].Id)) continue;
                    sprites[j] = new NamedSprite(sprites[j].Name, sprites[j].Sprite, _nextSpriteId);
                    usedSprites.Add(_nextSpriteId++);
                    characterChanged = true;
                }

                int id = character.Id;
                if (id <= 0 || !usedCharacters.Add(id)) {
                    id = _nextId++;
                    usedCharacters.Add(id);
                    characterChanged = true;
                }

                if (!characterChanged) continue;
                _characters[i] = new CharacterData(character.Name, sprites, character.DialogProfile, id);
                changed = true;
            }
            return changed;
        }

        private void OnValidate() {
            if (EnsureIds()) UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
