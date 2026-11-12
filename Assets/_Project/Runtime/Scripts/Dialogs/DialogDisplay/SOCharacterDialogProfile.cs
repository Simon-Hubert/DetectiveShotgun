using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;

namespace DShotgun.Dialogs
{
    [Serializable]
    public enum WritingProfile {
        CHAR_BY_CHAR = 0,
        WORD_BY_WORD = 1
    }
    
    [CreateAssetMenu(fileName = "SO_CharacterDialogProfile", menuName = "Scriptable Objects/Character/DialogProfile")]
    public class SOCharacterDialogProfile : ScriptableObject
    {
        [Header("Sprite & Visuals")]
        [SerializeField] private Sprite _textBoxBackground = null;
        public Sprite TextBoxSprite { get => _textBoxBackground; }
        [SerializeField] private bool _nameDisplayedAsTMP = true;
        [SerializeField] private Sprite _charaNameSprite = null;
        public bool NameDisplayedAsTMP { get => _nameDisplayedAsTMP; } 
        public Sprite CharaNameSprite { get => _charaNameSprite; }
        
        [Header("Typing")]
        [Tooltip("How the text will be typed")]
        [SerializeField] private WritingProfile _typingProfile = WritingProfile.CHAR_BY_CHAR;
        public WritingProfile TypingProfile { get => _typingProfile; }
        [Tooltip("Typing speed of the text in char/Sec\n0 = instant type")]
        [SerializeField, Min(0)] private float _typingSpeed = 20.0f;
        public float TypingSpeed { get => _typingSpeed; }
        public float TimeBetweenChar { get => _typingSpeed > 0.0f ? 1.0f / _typingSpeed : 0.0f; }
        
        [FormerlySerializedAs("defasultTypingSound")]
        [FormerlySerializedAs("_typingSound")]
        [Header("Sound")]
        [Tooltip("Sound played at each new char")]
        [SerializeField] private AudioClip _defasultTypingSound;
        public AudioClip DefaultTypingSound { get => _defasultTypingSound; }
        [Tooltip("Pitch of the Sound played")]
        [SerializeField, Range(-3.0f, 3.0f)] private float _soundPitch = 1.0f;
        public float SoundPitch { get => _soundPitch; }

        [FormerlySerializedAs("_hasDefaultColor")]
        [Header("Color")]
        [Tooltip("If there is a default text color for this character")]
        [SerializeField] private bool _hasPersonalisedColor = false;
        [SerializeField] private Color _defaultTextColor = Color.white;

        public Color DefaultTextColor(Color a_textColor)
        {
            if (_hasPersonalisedColor) {
                return _defaultTextColor;
            }
            return a_textColor;
        }

        [Header("Font")]
        [Tooltip("Font used by default by the character")]
        [SerializeField] private TMP_FontAsset _defaultFont = null;
        public TMP_FontAsset DefaultFont { get => _defaultFont; }
        [FormerlySerializedAs("_hasCaseStyle")]
        [Space(5)]
        [Tooltip("Style applied by default to the text\nSuperscript, Subscript & Highlights are ignored")]
        [SerializeField] private bool _hasFontStyle = false;
        [SerializeField] private FontStyles _fontStyle = 0;
        public FontStyles DefaultFontStyle(FontStyles a_fontStyle) {
            if (_hasFontStyle) {
                return _fontStyle;
            }
            return a_fontStyle;
        }

        [Header("Spacing")] 
        [Tooltip("If has default spacing for this character")]
        [SerializeField] private bool _hasPersonalisedSpacing = false;
        [SerializeField] private TextSpacing _textSpacing = new TextSpacing();
        public TextSpacing DefaultSpacing(TextSpacing a_spacing) {
            if (_hasPersonalisedSpacing) {
                return _textSpacing;
            }

            return a_spacing;
        }
        
        public CharacterDialogProfile ToStruct { get => new CharacterDialogProfile(this); }

        public SOCharacterDialogProfile CompleteDefaultProfile {
            set {
                if (_textBoxBackground == null) _textBoxBackground = value.TextBoxSprite;
                if (_charaNameSprite == null) _charaNameSprite = value.CharaNameSprite;
                _nameDisplayedAsTMP = value.NameDisplayedAsTMP;
                
                if (_defasultTypingSound == null) _defasultTypingSound = value.DefaultTypingSound;
                
                if (!_hasPersonalisedColor) _defaultTextColor = value.DefaultTextColor(value._defaultTextColor);
                if (_defaultFont == null) _defaultFont = value.DefaultFont;
                if (!_hasFontStyle) DefaultFontStyle(value._fontStyle);
                if (!_hasPersonalisedSpacing) DefaultSpacing(value._textSpacing);

                _hasPersonalisedColor = true;
                _hasFontStyle = true;
                _hasPersonalisedSpacing = true;
            }
        }
    }

    [Serializable]
    public struct CharacterDialogProfile
    {
        private SOCharacterDialogProfile _defaultProfile;
        public SOCharacterDialogProfile DefaultProfile { get => _defaultProfile; }
        
        public ProfileValues<WritingProfile> typingProfile;
        public ProfileValues<float> typingSpeed;
        public float typingDelay { get => typingSpeed.GetLastValue > 0.0f ? 1.0f / typingSpeed.GetLastValue : 0.0f; }
        public ProfileValues<AudioClip> typingSound;
        public ProfileValues<float> soundPitch;

        public void ResetAll() {
            typingProfile.ResetValues();
            typingSpeed.ResetValues();
            typingSound.ResetValues();
            soundPitch.ResetValues();
        }

        public CharacterDialogProfile(SOCharacterDialogProfile a_profile) {
            _defaultProfile = a_profile;
            
            typingProfile = new ProfileValues<WritingProfile>(_defaultProfile.TypingProfile);
            typingSpeed = new ProfileValues<float>(_defaultProfile.TypingSpeed);
            typingSound = new ProfileValues<AudioClip>(_defaultProfile.DefaultTypingSound);
            soundPitch = new ProfileValues<float>(_defaultProfile.SoundPitch);
        }
    }

    [Serializable]
    public struct ProfileValues<T>
    {
        [SerializeField, ReadOnly] private T _default;
        private List<T> queue;
        public void AddValue(T a_value) => queue.Add(a_value);
        public void RemoveValue() {
            if(queue.Count > 0) queue.RemoveAt(queue.Count - 1);
        } 
        
        public T GetLastValue { get => queue.Count <= 0 ? _default : queue[^1]; }
        public void ResetValues() {
            queue.Clear();
        }

        public ProfileValues(T a_type) {
            _default = a_type;
            
            queue = new List<T>();
        }
    }

    [Serializable]
    public struct TextSpacing
    {
        [Tooltip("Spacing between by characters")]
        public float character;
        [Tooltip("Spacing between by words")]
        public float word;
        [Tooltip("Spacing between by lines")]
        public float line;
        [Tooltip("Spacing between by paragraph")]
        public float paragraph;

        public TextSpacing(float a_character, float a_word, float a_line, float a_paragraph) {
            character = a_character;
            word = a_word;
            line = a_line;
            paragraph = a_paragraph;
        }
    }
}
