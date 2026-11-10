using System;

namespace DShotgun.RichText
{
    // Balise comprise par TextMeshPro : le compilateur vérifie sa valeur, la normalise et la laisse telle quelle pour TMP.
    // L'aperçu IMGUI ne connaît que b, i, color et size : le reste est imité (majuscules…) ou ignoré.
    public class TmpTag : RichTextTag
    {
        private const float PREVIEW_FONT_PX = 12f;   // taille de base du texte de l'inspecteur

        private readonly string _name;
        private readonly TmpValueKind _valueKind;
        private readonly TmpPreview _preview;
        private readonly TagClosing _closing;

        public TmpTag(string name, TmpValueKind valueKind = TmpValueKind.None, TmpPreview preview = TmpPreview.None, TagClosing closing = TagClosing.Optional) {
            _name = name;
            _valueKind = valueKind;
            _preview = preview;
            _closing = closing;
        }

        public override string Name => _name;
        public override TagClosing Closing => _closing;
        public override TagValueRule ValueRule => _valueKind == TmpValueKind.None ? TagValueRule.None : TagValueRule.Required;

        public override bool IsValueValid(string value, out string error) {
            error = null;
            switch (_valueKind)
            {
                case TmpValueKind.Color:
                    if (!TagValues.TryNormalizeColor(value, out _)) error = $"« {value} » n'est pas une couleur : {TagValues.ColorHelp}.";
                    break;
                case TmpValueKind.Percentage:
                    if (!TagValues.TryParsePercent(value, out _)) error = $"« {value} » n'est pas un pourcentage pour <{_name}> (ex : <{_name}=150%>).";
                    break;
                case TmpValueKind.Length:
                    if (!TagValues.TryNormalizeLength(value, out _)) error = $"« {value} » n'est pas une longueur pour <{_name}> (ex : <{_name}=2px> ou <{_name}=0.1em>).";
                    break;
            }
            return error == null;
        }

        public override string Open(RichTextTarget target, string value) {
            if (target == RichTextTarget.Plain) return string.Empty;
            if (target == RichTextTarget.Preview) return OpenPreview(value);
            return FormatOpen(Normalize(value));
        }

        public override string Close(RichTextTarget target) {
            if (target == RichTextTarget.Plain || _closing == TagClosing.None) return string.Empty;
            if (target == RichTextTarget.Preview) return ClosePreview();
            return FormatClose();
        }

        public override string TransformText(string text) {
            if (_preview == TmpPreview.Uppercase) return text.ToUpperInvariant();
            if (_preview == TmpPreview.Lowercase) return text.ToLowerInvariant();
            return text;
        }

        private string OpenPreview(string value) {
            switch (_preview)
            {
                case TmpPreview.Raw:
                case TmpPreview.Color:
                    return FormatOpen(Normalize(value));
                case TmpPreview.Size:
                    // L'IMGUI ne lit que des pixels, et TMP calcule les pourcentages sur la taille de base
                    TagValues.TryParsePercent(value, out float percent);
                    return $"<size={Math.Max(1, (int)Math.Round(PREVIEW_FONT_PX * percent / 100f))}>";
                case TmpPreview.LineBreak:
                    return "\n";
                default:
                    return string.Empty;
            }
        }

        private string ClosePreview() {
            switch (_preview)
            {
                case TmpPreview.Raw:
                case TmpPreview.Color:
                    return FormatClose();
                case TmpPreview.Size:
                    return "</size>";
                default:
                    return string.Empty;
            }
        }

        private string Normalize(string value) {
            if (value == null) return null;
            switch (_valueKind)
            {
                case TmpValueKind.Color:
                    return TagValues.TryNormalizeColor(value, out string color) ? color : value;
                case TmpValueKind.Percentage:
                    return TagValues.TryParsePercent(value, out float percent) ? TagValues.FormatInvariant(percent) + "%" : value;
                case TmpValueKind.Length:
                    return TagValues.TryNormalizeLength(value, out string length) ? length : value;
                default:
                    return value;
            }
        }
    }

    public enum TmpValueKind
    {
        None,         // <b>
        Text,         // <font=nom>
        Color,        // <color=#FF0000>
        Percentage,   // <size=150%>
        Length        // <cspace=2px>
    }

    public enum TmpPreview
    {
        None,        // sans équivalent dans l'IMGUI : invisible dans l'aperçu
        Raw,         // <b>, <i>
        Color,       // <color>
        Size,        // <size=150%> converti en pixels
        LineBreak,   // <br>
        Uppercase,   // le texte est mis en majuscules dans l'aperçu
        Lowercase
    }
}
