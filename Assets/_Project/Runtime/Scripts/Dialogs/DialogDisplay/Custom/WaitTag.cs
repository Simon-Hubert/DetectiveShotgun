using UnityEngine;

namespace DShotgun.Dialogs
{
    public class WaitTag : CustomTag
    {
        protected override string Name { get => "wait"; }
        protected override CustomTagsType Type { get => CustomTagsType.InstructionTag; }
        
        public override void ChangeInstructionValue(string a_tag, ref TextInstruction a_instruction) {
            if (TryGetValue<float>(a_tag, out float result)) {
                a_instruction.wait = result;
            }
            else {
                Debug.LogWarning($"'{Id.GetIdName}' warning : waiting time value wasn't recognized ({Id.GetTagValueRaw(a_tag)})");
            }
        }
    }
}
