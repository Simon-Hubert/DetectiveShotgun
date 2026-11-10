using System.Text;

namespace DShotgun.RichText
{
    // Balise raccourci : <big> = <size=200%>, <whisper> = <size=80%><color=grey><i>.
    // Elle s'écrit avec les balises TMP équivalentes, donc elle marche sans rien de plus dans le jeu.
    public class ExpandedTag : RichTextTag
    {
        private readonly string _name;
        private readonly (RichTextTag tag, string value)[] _parts;

        public ExpandedTag(string name, params (RichTextTag tag, string value)[] parts) {
            _name = name;
            _parts = parts;
        }

        public override string Name => _name;

        public override string Open(RichTextTarget target, string value) {
            StringBuilder opening = new();
            foreach ((RichTextTag tag, string partValue) in _parts) opening.Append(tag.Open(target, partValue));
            return opening.ToString();
        }

        public override string Close(RichTextTarget target) {
            StringBuilder closing = new();
            for (int i = _parts.Length - 1; i >= 0; i--) closing.Append(_parts[i].tag.Close(target));
            return closing.ToString();
        }

        public override string TransformText(string text) {
            foreach ((RichTextTag tag, string _) in _parts) text = tag.TransformText(text);
            return text;
        }
    }
}
