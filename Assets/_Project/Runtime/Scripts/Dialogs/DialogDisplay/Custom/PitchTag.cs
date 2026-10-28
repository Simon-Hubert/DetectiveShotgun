using UnityEngine;

namespace DShotgun.Dialogs
{
    public class PitchTag: CustomTag
    {
        protected override string Name { get => "pitch"; }
        protected override CustomTagsType Type { get => CustomTagsType.ProfileTag; }

        public override void ChangeProfileValue(string a_tag, ref CharacterDialogProfile a_profile) {
            if (Id.IsEndTag(a_tag)) {
                a_profile.soundPitch.RemoveValue();
            }
            else {
                if (TryGetValue<float>(a_tag, out float result)) {
                    a_profile.soundPitch.AddValue(result);
                }
            }
        }
    }
}
