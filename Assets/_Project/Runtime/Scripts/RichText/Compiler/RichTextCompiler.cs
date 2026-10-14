using System.Collections.Generic;

namespace DShotgun.RichText
{
    // Source → Lexer → Queue<Token> → Parser → Queue<Instruction> → Emitter → CompileResult
    
    /// <summary>
    /// 
    /// </summary>
    public static class RichTextCompiler
    {
        /// <summary>
        /// Compile le texte source en texte final, avec des balises, pour une cible (TMP, aperçu IMGUI, texte nu)
        /// </summary>
        /// <param name="source"> texte non compilé à traiter</param>
        /// <param name="target"> cible de la compilation (TMP, aperçu IMGUI, texte nu)</param>
        /// <param name="tags"> registre des balises disponibles</param>
        /// <returns> Renvoie le résultat de la compilation (le texte pret a etre affiché)</returns>
        public static CompileResult Compile(string source, RichTextTarget target, RichTextTagRegistry tags = null) {
            if (source == null) {
                source = string.Empty;
            }
            if (tags == null) {
                tags = RichTextTagRegistry.Default;
            }

            List<Diagnostic> diagnostics = new();
            List<TagSpan> spans = new();

            Queue<Token> tokens = RichTextLexer.Tokenize(source, tags, diagnostics);
            Queue<Instruction> instructions = RichTextParser.Parse(tokens, tags, diagnostics);
            string output = RichTextEmitter.Emit(instructions, target, spans);

            List<Diagnostic> located = new(diagnostics.Count);
            foreach (Diagnostic diagnostic in diagnostics) located.Add(diagnostic.WithLocation(source));
            located.Sort((a, b) => a.Position.CompareTo(b.Position));

            return new CompileResult(output, located, spans);
        }

        // Texte balisé corrigé (balises réparées, balises refusées retirées), sans le traitement propre à TMP
        public static string Correct(string source, RichTextTagRegistry tags = null) {
            return Compile(source, RichTextTarget.Tagged, tags).Output;
        }
    }

    /// <summary>
    /// Text traité pret a etre affiché
    /// </summary>
    public class CompileResult
    {
        public string Output { get; }
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public IReadOnlyList<TagSpan> Spans { get; }

        public bool HasErrors {
            get {
                foreach (Diagnostic diagnostic in Diagnostics) {
                    if (diagnostic.Level == DiagnosticLevel.Error) return true;
                }
                return false;
            }
        }

        public CompileResult(string output, IReadOnlyList<Diagnostic> diagnostics, IReadOnlyList<TagSpan> spans) {
            Output = output;
            Diagnostics = diagnostics;
            Spans = spans;
        }
    }

    /// <summary>
    /// Variable d'information sur le traitement du texte
    /// </summary>
    public readonly struct Diagnostic
    {
        public DiagnosticLevel Level { get; }
        public string Message { get; }
        public int Position { get; }
        public int Line { get; }       // à partir de 1, rempli par le compilateur
        public int Column { get; }

        private Diagnostic(DiagnosticLevel level, string message, int position, int line, int column) {
            Level = level;
            Message = message;
            Position = position;
            Line = line;
            Column = column;
        }

        public static Diagnostic Warning(int position, string message) => new(DiagnosticLevel.Warning, message, position, 0, 0);
        public static Diagnostic Error(int position, string message) => new(DiagnosticLevel.Error, message, position, 0, 0);

        internal Diagnostic WithLocation(string source) {
            int line = 1;
            int column = 1;
            int end = System.Math.Min(Position, source.Length);
            for (int i = 0; i < end; i++) {
                if (source[i] == '\n') {
                    line++;
                    column = 1;
                }
                else {
                    column++;
                }
            }
            return new Diagnostic(Level, Message, Position, line, column);
        }

        public override string ToString() => $"L{Line}:C{Column} {Level} : {Message}";
    }

    /// <summary>
    /// Type d'information
    /// </summary>
    public enum DiagnosticLevel
    {
        Warning,
        Error
    }

    /// <summary>
    /// Cible de la compilation
    /// </summary>
    public enum RichTextTarget
    {
        Tmp,        // TextMeshPro runtime
        Preview,    // aperçu inspectpr
        Plain,      // texte sans les tags
        Tagged      // texte balisé corrigé, sans échappement
    }
}
