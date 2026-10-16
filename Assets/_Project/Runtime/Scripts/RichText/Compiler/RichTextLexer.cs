using System.Collections.Generic;
using System.Text;

namespace DShotgun.RichText
{
    public static class RichTextLexer
    {
        /// <summary>
        /// Transforme les balises du text en tokens
        /// </summary>
        /// <param name="source"> Texte source </param>
        /// <param name="tags"> Dictionnaire des balises </param>
        /// <param name="diagnostics"> Liste des diagnostics </param>
        /// <returns></returns>
        public static Queue<Token> Tokenize(string source, RichTextTagRegistry tags, List<Diagnostic> diagnostics) {
            Queue<Token> tokens = new();
            if (string.IsNullOrEmpty(source)) return tokens;

            StringBuilder text = new();
            int textStart = 0;
            int i = 0;

            while (i < source.Length) {
                if (source[i] == '<' && TryReadTag(source, i, out string raw, out bool isClosing, out string name, out string value)) {
                    if (tags.TryGet(name, out _)) {
                        FlushText(tokens, text, textStart);
                        tokens.Enqueue(isClosing ? Token.Close(raw, name, i) : Token.Open(raw, name, value, i));
                        i += raw.Length;
                        continue;
                    }
                    diagnostics.Add(Diagnostic.Warning(i, $"Balise inconnue « {raw} » : gardée comme texte."));
                }

                if (text.Length == 0) textStart = i;
                text.Append(source[i]);
                i++;
            }

            FlushText(tokens, text, textStart);
            return tokens;
        }

        private static void FlushText(Queue<Token> tokens, StringBuilder text, int start) {
            if (text.Length == 0) return;
            tokens.Enqueue(Token.TextToken(text.ToString(), start));
            text.Clear();
        }
        
        private static bool TryReadTag(string source, int start, out string raw, out bool isClosing, out string name, out string value) {
            raw = null;
            isClosing = false;
            name = null;
            value = null;

            int end = source.IndexOf('>', start + 1);
            if (end < 0) return false;

            string inner = source.Substring(start + 1, end - start - 1);
            if (inner.IndexOf('<') >= 0) return false;

            isClosing = inner.StartsWith("/");
            if (isClosing) inner = inner.Substring(1);

            int equals = inner.IndexOf('=');
            name = equals < 0 ? inner : inner.Substring(0, equals);
            value = equals < 0 ? null : inner.Substring(equals + 1);

            if (!IsValidName(name) || (isClosing && value != null)) return false;

            name = name.ToLowerInvariant();
            raw = source.Substring(start, end - start + 1);
            return true;
        }

        private static bool IsValidName(string name) {
            if (name.Length == 0 || !char.IsLetter(name[0])) return false;
            foreach (char c in name) {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-') return false;
            }
            return true;
        }
    }

    public readonly struct Token
    {
        public TokenType Type { get; }
        public string Text { get; }      // texte brut
        public string TagName { get; }
        public string Value { get; }     // null si la balise est cassée
        public int Position { get; }     // index dans la source

        private Token(TokenType type, string text, string tagName, string value, int position) {
            Type = type;
            Text = text;
            TagName = tagName;
            Value = value;
            Position = position;
        }

        public static Token TextToken(string text, int position) => new(TokenType.Text, text, null, null, position);
        public static Token Open(string raw, string tagName, string value, int position) => new(TokenType.OpenTag, raw, tagName, value, position);
        public static Token Close(string raw, string tagName, int position) => new(TokenType.CloseTag, raw, tagName, null, position);
    }

    public enum TokenType
    {
        Text,
        OpenTag,
        CloseTag
    }
}
