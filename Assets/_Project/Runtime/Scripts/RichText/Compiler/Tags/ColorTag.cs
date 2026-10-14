namespace DShotgun.RichText
{
    public class ColorTag : RichTextTag
    {
        public override string Name => "color";
        public override TagValueRule ValueRule => TagValueRule.Required;
        
        public override bool IsValueValid(string value, out string error) {
            bool valid = value.Length is 7 or 9 && value[0] == '#'; //#RRGGBB ou #RRGGBBAA
            for (int i = 1; valid && i < value.Length; i++) {
                valid = System.Uri.IsHexDigit(value[i]);
            }
            error = valid ? null : $"Couleur « {value} » invalide : attendu #RRGGBB ou #RRGGBBAA.";
            return valid;
        }

        public override string Open(RichTextTarget target, string value) =>
            target == RichTextTarget.Plain ? string.Empty : FormatOpen(value);

        public override string Close(RichTextTarget target) =>
            target == RichTextTarget.Plain ? string.Empty : FormatClose();
    }
}
