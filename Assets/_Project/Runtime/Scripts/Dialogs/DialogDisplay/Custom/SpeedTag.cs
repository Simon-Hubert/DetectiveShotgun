using UnityEngine;

namespace DShotgun.Dialogs
{
    public class SpeedTag : CustomTag
    {
        protected override string Name { get => "speed"; }
        
        public override void ChangeProfileValue(string a_tag, ref CharacterDialogProfile a_profile) {
            if (Id.IsEndTag(a_tag)) {
                a_profile.typingSpeed.RemoveValue();
            }
            else {
                if (TryGetValue<float>(a_tag, out float result)) {
                    a_profile.typingSpeed.AddValue(result);
                }
            }
        }
    }
}
