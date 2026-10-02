using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

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
        [Tooltip("How the text will be typed")]
        [SerializeField] private WritingProfile _typingProfile = WritingProfile.CHAR_BY_CHAR;
        public WritingProfile TypingProfile { get => _typingProfile; }
        [Tooltip("Typing speed of the text in char/Sec\n0 = instant type")]
        [SerializeField, Min(0)] private float _typingSpeed = 20.0f;
        public float TypingSpeed { get => _typingSpeed; }
        public float TimeBetweenChar { get => _typingSpeed > 0.0f ? 1.0f / _typingSpeed : 0.0f; }
        
        [Header("Sound")]
        [Tooltip("Sound played at each new char")]
        [SerializeField] private AudioClip _typingSound;
        public AudioClip TypingSound { get => _typingSound; }
        [Tooltip("Pitch of the Sound played")]
        [SerializeField, Range(-3.0f, 3.0f)] private float _soundPitch = 1.0f;
        public float SoundPitch { get => _soundPitch; }

        [FormerlySerializedAs("_hasDefaultColor")]
        [Header("Color")]
        [Tooltip("If there is a default text color for this character")]
        [SerializeField] private bool _hasPersonalisedColor = false;
        [SerializeField] private Color _defaultTextColor = Color.white;
        public bool DefaultTextColor(out Color o_color) {
            if (_hasPersonalisedColor) {
                o_color = _defaultTextColor;
                return true;
            }
            else {
                o_color = Color.deepPink;
                return false;
            }
        }

        [Header("Font")]
        [Tooltip("Font used by default by the character")]
        [SerializeField] private TMP_FontAsset _defaultFont = null;
        public TMP_FontAsset DefaultFont { get => _defaultFont; }
        [Tooltip("Style applied by default to the text\nSuperscript, Subscript & Highlights are ignored")]
        [SerializeField] private FontStyles _caseStyle = 0;
        public TMP_FontStyleStack DefaultFontStyle {
            get {
                TMP_FontStyleStack style = new TMP_FontStyleStack();
                
                if ((_caseStyle & FontStyles.Bold) != 0) style.Add(FontStyles.Bold);
                if ((_caseStyle & FontStyles.Italic) != 0) style.Add(FontStyles.Italic);
                if ((_caseStyle & FontStyles.Underline) != 0) style.Add(FontStyles.Underline);
                if ((_caseStyle & FontStyles.Strikethrough) != 0) style.Add(FontStyles.Strikethrough);
                if ((_caseStyle & FontStyles.LowerCase) != 0) style.Add(FontStyles.LowerCase);
                if ((_caseStyle & FontStyles.UpperCase) != 0) style.Add(FontStyles.UpperCase);
                if ((_caseStyle & FontStyles.SmallCaps) != 0) style.Add(FontStyles.SmallCaps);

                return style;
            }
            
        }

        [Header("Spacing")] 
        [Tooltip("If has default spacing for this character")]
        [SerializeField] private bool _hasPersonalisedSpacing = false;
        [Tooltip("Spacing between by characters")]
        [SerializeField] private float _defaultSpacingCharacter;
        [Tooltip("Spacing between by words")]
        [SerializeField] private float _defaultSpacingWord;
        [Tooltip("Spacing between by lines")]
        [SerializeField] private float _defaultSpacingLine;
        [Tooltip("Spacing between by paragraph")]
        [SerializeField] private float _defaultSpacingParagraph;
        public bool DefaultSpacing(out float o_char, out float o_word, out float o_line, out float o_para) {
            if (_hasPersonalisedSpacing) {
                o_char = _defaultSpacingCharacter;
                o_word = _defaultSpacingWord;
                o_line = _defaultSpacingLine;
                o_para = _defaultSpacingParagraph;
                return true;
            }
            else {
                o_char = 0;
                o_word = 0;
                o_line = 0;
                o_para = 0;
                return false;
            }
        }
    }

    [Serializable]
    public struct CharacterDialogProfile
    {
        private SOCharacterDialogProfile _defaultDialogProfile;
        public SOCharacterDialogProfile DefaultDialogProfile { get => _defaultDialogProfile; }
        
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
            _defaultDialogProfile = a_profile;
            
            typingProfile = new ProfileValues<WritingProfile>(_defaultDialogProfile.TypingProfile);
            typingSpeed = new ProfileValues<float>(_defaultDialogProfile.TypingSpeed);
            typingSound = new ProfileValues<AudioClip>(_defaultDialogProfile.TypingSound);
            soundPitch = new ProfileValues<float>(_defaultDialogProfile.SoundPitch);
        }
    }

    [Serializable]
    public struct ProfileValues<T>
    {
        private T _default;
        public List<T> queue;
        
        public T GetLastValue { get => queue[^1]; }
        public void ResetValues() {
            queue = new List<T>();
            queue.Add(_default);
        }

        public ProfileValues(T a_type) {
            _default = a_type;
            
            queue = new List<T>();
            queue.Add(default);
        }
    }
}
