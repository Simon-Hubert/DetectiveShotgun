using System.Collections.Generic;

namespace DShotgun.RichText
{
    /// <summary>
    /// Analyse une séquence de Token et répare les balises cassées si possible
    /// </summary>
    public static class RichTextParser
    {
        /// <summary>
        /// Parse une séquence de <see cref="Token"/> et produit une séquence d'instructions pour l'émetteur.
        /// </summary>
        /// <param name="tokens"> Balise traitée par le <see cref="RichTextLexer"/></param>
        /// <param name="tags"> Dictionnaire des balises </param>
        /// <param name="diagnostics"> Liste des diagnostics </param>
        /// <returns> Renvoie la queue d'instructions pour l'émetteur</returns>
        public static Queue<Instruction> Parse(Queue<Token> tokens, RichTextTagRegistry tags, List<Diagnostic> diagnostics) {
            Queue<Instruction> instructions = new();
            Stack<Token> openTags = new();   // la dernière ouverte doit être la première fermée
            Dictionary<string, int> ignoredOpenings = new();   // balises refusées à l'ouverture : leur fermeture est ignorée sans soucis

            while (tokens.Count > 0) 
            {
                Token token = tokens.Dequeue();
                switch (token.Type) 
                {
                    case TokenType.Text:
                        instructions.Enqueue(Instruction.TextInstruction(token.Text));
                        break;
                    case TokenType.OpenTag:
                        OpenTag(token, tags, openTags, ignoredOpenings, instructions, diagnostics);
                        break;
                    case TokenType.CloseTag:
                        CloseTag(token, tags, openTags, ignoredOpenings, instructions, diagnostics);
                        break;
                }
            }

            while (openTags.Count > 0) {
                Token unclosed = openTags.Pop();
                diagnostics.Add(Diagnostic.Error(unclosed.Position, $"« {unclosed.Text} » n'est jamais fermée : fermée automatiquement en fin de texte."));
                instructions.Enqueue(Instruction.Pop(GetTag(tags, unclosed)));
            }

            return instructions;
        }

        private static void OpenTag(Token token, RichTextTagRegistry tags, Stack<Token> openTags,
            Dictionary<string, int> ignoredOpenings, Queue<Instruction> instructions, List<Diagnostic> diagnostics) 
        {
            RichTextTag tag = GetTag(tags, token);
            string value = token.Value;

            if (tag.ValueRule == TagValueRule.Required && string.IsNullOrEmpty(value))
            {
                diagnostics.Add(Diagnostic.Error(token.Position, $"« {token.Text} » demande une valeur : <{tag.Name}=…>. Balise ignorée."));
                Ignore(ignoredOpenings, token.TagName);
                return;
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
                return;
            }
            
            openTags.Push(token);
            instructions.Enqueue(Instruction.Push(tag, value));
        }

        private static void CloseTag(Token token, RichTextTagRegistry tags, Stack<Token> openTags,
            Dictionary<string, int> ignoredOpenings, Queue<Instruction> instructions, List<Diagnostic> diagnostics) {
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

            Token top = openTags.Peek();
            if (top.TagName != token.TagName) 
            {
                diagnostics.Add(Diagnostic.Error(token.Position,
                    $"« {token.Text} » ferme dans le désordre : « {top.Text} » est encore ouverte. Réparé."));
            }

            // Ferme les balises intermédiaires puis celle demandée
            while (true) 
            {
                Token closed = openTags.Pop();
                instructions.Enqueue(Instruction.Pop(GetTag(tags, closed)));
                if (closed.TagName == token.TagName) break;
            }
        }

        private static void Ignore(Dictionary<string, int> ignoredOpenings, string tagName) 
        {
            ignoredOpenings.TryGetValue(tagName, out int count);
            ignoredOpenings[tagName] = count + 1;
        }

        private static bool IsOpen(Stack<Token> openTags, string tagName) 
        {
            foreach (Token open in openTags) 
            {
                if (open.TagName == tagName) return true;
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
