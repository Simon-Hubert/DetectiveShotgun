using System;
using System.Globalization;
using System.Linq;

namespace DShotgun.RichText
{
    /// <summary>
    /// Valise lue au runtime par le TextDumbReader : le compilateur la vérifie et la garde telle quelle dans le texte du dialogue.
    /// </summary>
    public class DialogueTag : RichTextTag
    {
        private const string MARKER_FORMAT = "<color=#8A8A8A>[{0}]</color>";

        private readonly string _name;
        private readonly TagClosing _closing;
        private readonly DialogueValueKind _valueKind;
        private readonly string _previewOpen;    // {0} = la valeur de la balise
        private readonly string _previewClose;
        private readonly bool _runtimeSupported;
        private readonly bool _endsDialogue;
        private readonly string[] _choices;

        public DialogueTag(string name, TagClosing closing, DialogueValueKind valueKind, string previewOpen = null, string previewClose = null,
            bool runtimeSupported = true, bool endsDialogue = false, string[] choices = null) {
            _name = name;
            _closing = closing;
            _valueKind = valueKind;
            _previewOpen = previewOpen;
            _previewClose = previewClose;
            _runtimeSupported = runtimeSupported;
            _endsDialogue = endsDialogue;
            _choices = choices;
        }

        /// <summary>
        /// Repère grisé dans l'aperçu, pour les balises autonomes
        /// </summary>
        /// <param name="text">le texte a format</param>
        /// <returns>le text marqué</returns>
        public static string Marker(string text) => string.Format(MARKER_FORMAT, text);

        public override string Name => _name;
        public override TagClosing Closing => _closing;
        public override bool IsRuntimeSupported => _runtimeSupported;
        public override bool EndsDialogue => _endsDialogue;
        public override TagValueRule ValueRule => _valueKind == DialogueValueKind.None ? TagValueRule.None : TagValueRule.Required;

        public override bool IsValueValid(string value, out string error) {
            error = null;
            if (_valueKind == DialogueValueKind.PositiveNumber && !TagValues.TryParsePositive(value, out _)) {
                error = $"« {value} » n'est pas un nombre positif pour <{_name}> (ex : <{_name}=1.5>).";
            }
            else if (_valueKind == DialogueValueKind.Choice && !_choices.Contains(value, StringComparer.OrdinalIgnoreCase)) {
                error = $"« {value} » n'est pas une valeur de <{_name}> : {string.Join(" ou ", _choices)}.";
            }
            return error == null;
        }

        public override string Open(RichTextTarget target, string value) {
            if (target == RichTextTarget.Tagged) {
                if (!_runtimeSupported) return string.Empty;
                return FormatOpen(Normalize(value));
            }
            if (target == RichTextTarget.Preview && _previewOpen != null) return string.Format(_previewOpen, value);
            return string.Empty;
        }

        public override string Close(RichTextTarget target) {
            if (_closing == TagClosing.None) return string.Empty;
            if (target == RichTextTarget.Tagged && _runtimeSupported) return FormatClose();
            if (target == RichTextTarget.Preview && _previewClose != null) return _previewClose;
            return string.Empty;
        }

        private string Normalize(string value) {
            if (value == null) return null;
            // Le TextDumbReader lit avec float.TryParse, donc avec le naming du système (1,5 en français) :
            if (_valueKind == DialogueValueKind.PositiveNumber && TagValues.TryParsePositive(value, out float number)) {
                return number.ToString(CultureInfo.CurrentCulture);
            }
            if (_valueKind == DialogueValueKind.Choice) return value.ToLowerInvariant();
            return value;
        }
    }

    public enum DialogueValueKind
    {
        None,             // <pass>
        PositiveNumber,   // <wait=0.5>
        Text,             // <sound=nom>
        Choice            // <readmod=word>
    }
}
