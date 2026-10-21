using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Threading;

namespace DShotgun.Dialogs
{
    public class TextDumbReader
    {
        private TextMeshProUGUI _textBox;
        
        public async Awaitable ComputeCharByCharAsync(string a_text, SOCharacterDialogProfile a_profile, CancellationToken a_cancel) {
            CharacterDialogProfile profileStruct = a_profile.ToStruct;

            try {
                float delay = Time.deltaTime;
                _textBox.text = "";
                StringBuilder a = new StringBuilder(_textBox.text);

                for (int i = 0; i < a_text.Length; i++) {
                    if (a_text[i] == '<') a.Append(TagsTreatment(a_text, ref profileStruct, ref i));
                    if (string.IsNullOrWhiteSpace(a_text[i].ToString())) a.Append(a_text[i]);
                    else {
                        while (profileStruct.typingDelay > delay) {
                            await Awaitable.NextFrameAsync(a_cancel);
                            delay += Time.deltaTime;
                        }

                        delay -= profileStruct.typingDelay;

                        a.Append(a_text[i]);
                        _textBox.text = a.ToString();
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"'DumbReader' : TextTreatment Async was cancelled - {e}");
            }
            finally {
                
            }
        }
        
        private string TagsTreatment(string a_text, ref CharacterDialogProfile a_profile, ref int r_index) {
            StringBuilder tagSB = new StringBuilder("<");
            
            for (r_index = r_index + 1; r_index < a_text.Length; r_index++) {
                tagSB.Append(a_text[r_index]);

                if (a_text[r_index] == '>') {
                    r_index++;
                    break;
                }
            }

            string tag = tagSB.ToString();

            if (tag[^1] == '>') {
                //Check custom tag

                return tag;
            }
            else {
                return "";
            }
        }

        public TextDumbReader() {
            _textBox = null;
        }
        public TextDumbReader(TextMeshProUGUI a_textBox) {
            _textBox = a_textBox;
        }
    }
}
