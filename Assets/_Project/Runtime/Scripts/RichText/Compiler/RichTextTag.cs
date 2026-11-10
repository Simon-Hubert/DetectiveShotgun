namespace DShotgun.RichText
{
    // ajoutée dans RichTextTagRegistry.CreateDefault().
    public abstract class RichTextTag
    {
        public abstract string Name { get; }    // en minuscules, sans chevrons "b" àr exemple
        public virtual TagValueRule ValueRule => TagValueRule.None;
        public virtual TagClosing Closing => TagClosing.Optional;
        public virtual bool IsRuntimeSupported => true;   // false : le jeu ne gère pas encore la balise, elle est retirée du texte
        public virtual bool EndsDialogue => false;        // true : le texte écrit après n'est jamais affiché (<pass>)

        // Contrôle le format de la valeur. Renvoyer false = balise ignorée (erreur), le texte est gardé
        public virtual bool IsValueValid(string value, out string error) {
            error = null;
            return true;
        }
        
        public virtual string Open(RichTextTarget target, string value) =>
            target == RichTextTarget.Tmp || target == RichTextTarget.Tagged ? FormatOpen(value) : string.Empty;

        public virtual string Close(RichTextTarget target) =>
            target == RichTextTarget.Tmp || target == RichTextTarget.Tagged ? FormatClose() : string.Empty;

        // Appelée sur chaque morceau de texte tant que la balise est ouverte (aperçu seulement : TMP s'en charge en jeu)
        public virtual string TransformText(string text) => text;

        protected string FormatOpen(string value) => value == null ? $"<{Name}>" : $"<{Name}={value}>";
        protected string FormatClose() => $"</{Name}>";
    }

    public enum TagClosing
    {
        Optional,   // Encadrant : <b>…</b>. Sans fermeture, la balise reste active jusqu'à la fin du texte
        None        // Autonome : <wait=0.5>
    }

    public enum TagValueRule
    {
        None,       // <b>
        Required,   // <color=#FF0000> AVEC DES VALUES
    }
}
