namespace DShotgun.RichText
{
    // ajoutée dans RichTextTagRegistry.CreateDefault().
    public abstract class RichTextTag
    {
        public abstract string Name { get; }    // en minuscules, sans chevrons "b" àr exemple
        public virtual TagValueRule ValueRule => TagValueRule.None;

        // Contrôle le format de la valeur. Renvoyer false = balise ignorée (erreur), le texte est gardé
        public virtual bool IsValueValid(string value, out string error) {
            error = null;
            return true;
        }
        
        public virtual string Open(RichTextTarget target, string value) =>
            target == RichTextTarget.Tmp || target == RichTextTarget.Tagged ? FormatOpen(value) : string.Empty;

        public virtual string Close(RichTextTarget target) =>
            target == RichTextTarget.Tmp || target == RichTextTarget.Tagged ? FormatClose() : string.Empty;

        protected string FormatOpen(string value) => value == null ? $"<{Name}>" : $"<{Name}={value}>";
        protected string FormatClose() => $"</{Name}>";
    }

    public enum TagValueRule
    {
        None,       // <b>
        Required,   // <color=#FF0000>
    }
}
