using UnityEngine;

namespace DShotgun.Dialogs
{
    public class SoundTag : CustomTag
    {
        protected override string Name { get => "sound"; }
        
        public override void ChangeProfileValue(string a_tag, ref CharacterDialogProfile a_profile) {
            if (Id.IsEndTag(a_tag)) {
                a_profile.typingSound.RemoveValue();
            }
            else {
                if (TryGetValue<string>(a_tag, out string result)) {
                    
                    //a_profile.typingSound.AddValue();
                }
            }
        }
    }
}
