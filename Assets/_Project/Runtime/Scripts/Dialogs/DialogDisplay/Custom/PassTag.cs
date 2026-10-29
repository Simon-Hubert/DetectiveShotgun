using UnityEngine;

namespace DShotgun.Dialogs
{
    public class PassTag : CustomTag
    {
        protected override string Name { get => "Pass"; }
        protected override CustomTagsType Type { get => CustomTagsType.InstructionTag; }

        public override void ChangeInstructionValue(string a_tag, ref TextInstruction a_instruction) {
            a_instruction.pass = true;
        }
    }
}
