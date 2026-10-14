namespace DShotgun.RichText
{
    public class ItalicTag : RichTextTag
    {
        public override string Name => "i";

        public override string Open(RichTextTarget target, string value) =>
            target == RichTextTarget.Plain ? string.Empty : FormatOpen(null);

        public override string Close(RichTextTarget target) =>
            target == RichTextTarget.Plain ? string.Empty : FormatClose();
    }
}
