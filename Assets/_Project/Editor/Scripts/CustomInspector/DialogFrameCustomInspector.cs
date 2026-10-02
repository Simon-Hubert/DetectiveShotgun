using System;
using System.Collections.Generic;
using System.Linq;
using DShotgun.Dialogs;
using DShotgun;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{
    [CustomEditor(typeof(DialogFrame))]
    public class DialogFrameCustomInspector : UnityEditor.Editor
    {
        private int _leftSelectedName = 0;
        private int _middleSelectedName = 0;
        private int _rightSelectedName = 0;

        private int _leftSelectedSprite = 0;
        private int _middleSelectedSprite = 0;
        private int _rightSelectedSprite = 0;

        private IReadOnlyList<NamedSprite> _leftSprites;
        private IReadOnlyList<NamedSprite> _middleSprites;
        private IReadOnlyList<NamedSprite> _rightSprites;
        
        private DialogFrame _dialogFrame;
        private CharactersDatabase _charactersDatabase;
        private string[] _names;

        public bool _showAdditionalSettings = false;
        
        public override void OnInspectorGUI() {
            _charactersDatabase ??= Resources.Load<CharactersDatabase>("Characters");
            _dialogFrame = target as DialogFrame;
            _names = _charactersDatabase.Characters.Select(e => e.Name).Prepend("None").ToArray();
            
            GetCharacterNames();
            if (_dialogFrame) {
                //Read Current state by name
                _leftSelectedSprite = GetSprite(out _leftSprites, _leftSelectedName, _dialogFrame.LeftRef);
                _middleSelectedSprite = GetSprite(out _middleSprites, _middleSelectedName, _dialogFrame.MiddleRef);
                _rightSelectedSprite = GetSprite(out _rightSprites, _rightSelectedName, _dialogFrame.RightRef);

                GUILayout.BeginHorizontal();
                //Change via Editor
                CharacterPopup(ref _leftSelectedName, ref _leftSelectedSprite, _leftSprites, out DialogFrame.SpriteRef leftFrame, out Sprite leftSprite,0);
                CharacterPopup(ref _middleSelectedName, ref _middleSelectedSprite, _middleSprites, out DialogFrame.SpriteRef middleFrame, out Sprite middleSprite,1);
                CharacterPopup(ref _rightSelectedName, ref _rightSelectedSprite, _rightSprites, out DialogFrame.SpriteRef rightFrame, out Sprite rightSprite,2);
                
                GUILayout.EndHorizontal();

                // Set New State by name
                _dialogFrame.LeftRef = leftFrame;
                _dialogFrame.Left = leftSprite;
                _dialogFrame.MiddleRef = middleFrame;
                _dialogFrame.Middle = middleSprite;
                _dialogFrame.RightRef = rightFrame;
                _dialogFrame.Right= rightSprite;
                GUILayout.Space(10);

                _dialogFrame.MoveNextAutomatically = GUILayout.Toggle(
                    _dialogFrame.MoveNextAutomatically, "Move to next Frame immediately after");

                _showAdditionalSettings = EditorGUILayout.Foldout(_showAdditionalSettings, "Show Additional Settings");
                if (_showAdditionalSettings) {
                    bool noOneSpeaking = GUILayout.Toggle(_dialogFrame.speakingCharacterID == -1, "Don't Show Speaker");
                    _dialogFrame.UseCustomName = GUILayout.Toggle(_dialogFrame.UseCustomName, "Use Custom Name");
                    if (_dialogFrame.UseCustomName) {
                        _dialogFrame.Name = GUILayout.TextField(_dialogFrame.Name);
                    }
                
                    _dialogFrame.UseCustomDialogProfile = GUILayout.Toggle(_dialogFrame.UseCustomDialogProfile, "Use Custom Dialog Profile");
                    if (_dialogFrame.UseCustomDialogProfile) {
                        _dialogFrame.CharacterDialogProfile = (SOCharacterDialogProfile)EditorGUILayout.ObjectField(_dialogFrame.CharacterDialogProfile, typeof(SOCharacterDialogProfile));
                    }
                
                    if (noOneSpeaking) {
                        _dialogFrame.speakingCharacterID = -1;
                        if (!_dialogFrame.UseCustomName) {
                            _dialogFrame.Name = "";
                        }
                    }
                }
                
                GUILayout.Space(10);
                _dialogFrame.Text = GUILayout.TextArea(_dialogFrame.Text, GUILayout.Height(100));
            }
        }
        
        private void GetCharacterNames() {
            for (int i = 0; i < _names.Length; i++) {
                if (_dialogFrame.LeftRef.characterName == _names[i]) _leftSelectedName = i;
                if (_dialogFrame.MiddleRef.characterName == _names[i]) _middleSelectedName = i;
                if (_dialogFrame.RightRef.characterName == _names[i]) _rightSelectedName = i;
            }
        }
        
        private int GetSprite(out IReadOnlyList<NamedSprite> sprites, int characterNameID, DialogFrame.SpriteRef refTuple) {
            if (characterNameID == 0) {
                sprites = null;
                return 0;
            }
            sprites = _charactersDatabase.GetCharacter(_names[characterNameID]).SpriteList;
            for (int i = 0; i < sprites.Count; i++) {
                if (sprites[i].Name == refTuple.spriteName) return i;
            }
            return 0;
        }

        private void CharacterPopup(ref int selectedName, ref int selectedSprite, IReadOnlyList<NamedSprite> sprites, out DialogFrame.SpriteRef frame, out Sprite sprite, int characterID) {
            GUILayout.BeginVertical();
            selectedName = EditorGUILayout.Popup(selectedName, _names);
            selectedSprite = sprites == null ? 0 : EditorGUILayout.Popup(selectedSprite, sprites.Select(e => e.Name).ToArray());
            
            frame = new DialogFrame.SpriteRef();
            frame.characterName = _names[selectedName];
            if (sprites == null) {
                frame.spriteName = "None";
                sprite = null;
            }
            else {
                frame.spriteName = sprites[selectedSprite].Name;
                sprite = sprites[selectedSprite].Sprite;
            }

            if (GUILayout.Toggle(characterID == _dialogFrame.speakingCharacterID, "Speaking")) {
                _dialogFrame.speakingCharacterID = characterID;
                if(!_dialogFrame.UseCustomDialogProfile) _dialogFrame.CharacterDialogProfile = _charactersDatabase.GetCharacter(_names[selectedName]).DialogProfile;
                if(!_dialogFrame.UseCustomName) _dialogFrame.Name = _names[selectedName];
            }
            GUILayout.EndVertical();
        }
    }
}
