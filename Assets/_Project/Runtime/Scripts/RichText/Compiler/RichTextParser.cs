using System.Collections.Generic;

namespace DShotgun.RichText
{
    /// <summary>
    /// Analyse une séquence de Token et répare les balises cassées si possible
    /// </summary>
    public static class RichTextParser
    {
        private readonly struct OpenEntry
        {
            public readonly Token Token;
            public readonly RichTextTag Tag;
            public readonly string Value;   // valeur déjà validée, rejouée si la balise doit être rouverte

            public OpenEntry(Token token, RichTextTag tag, string value) {
                Token = token;
                Tag = tag;
                Value = value;
            }
        }

        /// <summary>
        /// Parse une séquence de <see cref="Token"/> et produit une séquence d'instructions pour l'émetteur.
        /// </summary>
        /// <param name="tokens"> Balise traitée par le <see cref="RichTextLexer"/></param>
        /// <param name="tags"> Dictionnaire des balises </param>
        /// <param name="diagnostics"> Liste des diagnostics </param>
        /// <returns> Renvoie la queue d'instructions pour l'émetteur</returns>
        public static Queue<Instruction> Parse(Queue<Token> tokens, RichTextTagRegistry tags, List<Diagnostic> diagnostics) {
            Queue<Instruction> instructions = new();
            Stack<OpenEntry> openTags = new();   // la dernière ouverte est la première fermée
            Dictionary<string, int> ignoredOpenings = new();   // balises refusées à l'ouverture : leur fermeture est ignorée sans soucis
            bool dialogueEnded = false;   // un <pass> a été lu : le texte qui suit n'est jamais affiché
            bool warnedAfterEnd = false;

            while (tokens.Count > 0)
            {
                Token token = tokens.Dequeue();
                switch (token.Type)
                {
                    case TokenType.Text:
                        if (dialogueEnded && !warnedAfterEnd && !string.IsNullOrWhiteSpace(token.Text))
                        {
                            diagnostics.Add(Diagnostic.Warning(token.Position, "Ce texte ne sera pas affiché : <pass> passe automatiquement au dialogue suivant."));
                            warnedAfterEnd = true;
                        }
                        instructions.Enqueue(Instruction.TextInstruction(token.Text));
                        break;
                    case TokenType.OpenTag:
                        RichTextTag opened = OpenTag(token, tags, openTags, ignoredOpenings, instructions, diagnostics);
                        if (opened != null && opened.EndsDialogue) dialogueEnded = true;
                        break;
                    case TokenType.CloseTag:
                        CloseTag(token, openTags, ignoredOpenings, instructions, diagnostics);
                        break;
                }
            }

            // Une balise encadrante non fermée reste active jusqu'à la fin : on la ferme sans erreur
            while (openTags.Count > 0) {
                instructions.Enqueue(Instruction.Pop(openTags.Pop().Tag));
            }

            return instructions;
        }

        // Renvoie la balise ouverte, ou null si elle a été refusée
        private static RichTextTag OpenTag(Token token, RichTextTagRegistry tags, Stack<OpenEntry> openTags,
            Dictionary<string, int> ignoredOpenings, Queue<Instruction> instructions, List<Diagnostic> diagnostics)
        {
            RichTextTag tag = GetTag(tags, token);
            string value = token.Value;

            if (tag.ValueRule == TagValueRule.Required && string.IsNullOrEmpty(value))
            {
                diagnostics.Add(Diagnostic.Error(token.Position, $"« {token.Text} » demande une valeur : <{tag.Name}=…>. Balise ignorée."));
                Ignore(ignoredOpenings, token.TagName);
                return null;
            }
            if (tag.ValueRule == TagValueRule.None && value != null)
            {
                diagnostics.Add(Diagnostic.Warning(token.Position, $"« {token.Text} » ne prend pas de valeur : « {value} » est ignorée."));
                value = null;
            }
            if (value != null && !tag.IsValueValid(value, out string error))
            {
                diagnostics.Add(Diagnostic.Error(token.Position, error + " Balise ignorée."));
                Ignore(ignoredOpenings, token.TagName);
                return null;
            }
            if (!tag.IsRuntimeSupported)
            {
                diagnostics.Add(Diagnostic.Warning(token.Position, $"« {token.Text} » n'est pas encore gérée par le jeu : elle est sans effet."));
            }

            instructions.Enqueue(Instruction.Push(tag, value));
            if (tag.Closing == TagClosing.None)
            {
                instructions.Enqueue(Instruction.Pop(tag));   // balise autonome : rien à fermer
                return tag;
            }

            openTags.Push(new OpenEntry(token, tag, value));
            return tag;
        }

        private static void CloseTag(Token token, Stack<OpenEntry> openTags, Dictionary<string, int> ignoredOpenings,
            Queue<Instruction> instructions, List<Diagnostic> diagnostics) {
            if (!IsOpen(openTags, token.TagName))
            {
                if (ignoredOpenings.TryGetValue(token.TagName, out int count) && count > 0)
                {
                    ignoredOpenings[token.TagName] = count - 1;
                    return;
                }
                diagnostics.Add(Diagnostic.Error(token.Position, $"« {token.Text} » ferme une balise qui n'est pas ouverte : ignorée."));
                return;
            }
            
            Stack<OpenEntry> toReopen = new();
            while (true)
            {
                OpenEntry closed = openTags.Pop();
                instructions.Enqueue(Instruction.Pop(closed.Tag));
                if (closed.Token.TagName == token.TagName) break;
                toReopen.Push(closed);
            }

            while (toReopen.Count > 0)
            {
                OpenEntry reopened = toReopen.Pop();
                instructions.Enqueue(Instruction.Push(reopened.Tag, reopened.Value));
                openTags.Push(reopened);
            }
        }

        private static void Ignore(Dictionary<string, int> ignoredOpenings, string tagName)
        {
            ignoredOpenings.TryGetValue(tagName, out int count);
            ignoredOpenings[tagName] = count + 1;
        }

        private static bool IsOpen(Stack<OpenEntry> openTags, string tagName)
        {
            foreach (OpenEntry open in openTags)
            {
                if (open.Token.TagName == tagName) return true;
            }
            return false;
        }

        // Le lexer ne produit que des balises connues : la recherche réussit toujours ici.
        private static RichTextTag GetTag(RichTextTagRegistry tags, Token token)
        {
            tags.TryGet(token.TagName, out RichTextTag tag);
            return tag;
        }
    }

    public readonly struct Instruction
    {
        public InstructionType Type { get; }
        public string Text { get; }
        public RichTextTag Tag { get; }
        public string Value { get; }

        private Instruction(InstructionType type, string text, RichTextTag tag, string value) {
            Type = type;
            Text = text;
            Tag = tag;
            Value = value;
        }

        public static Instruction TextInstruction(string text) => new(InstructionType.Text, text, null, null);
        public static Instruction Push(RichTextTag tag, string value) => new(InstructionType.Push, null, tag, value);
        public static Instruction Pop(RichTextTag tag) => new(InstructionType.Pop, null, tag, null);
    }

    public enum InstructionType
    {
        Text,
        Push,
        Pop
    }
}
