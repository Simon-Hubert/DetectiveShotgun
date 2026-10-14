using System;

namespace DShotgun.Editor
{
    public static class RichTextTagInserter
    {
        // Entoure [start, end[ de balises. La nouvelle sélection couvre le même texte, à l'intérieur des balises.
        // Sans sélection (start == end), insère une paire vide avec le curseur au milieu.
        public static string Wrap(string text, int start, int end, string open, string close, out int newStart, out int newEnd) {
            if (text == null) {
                text = string.Empty;
            }
            if (start > end) (start, end) = (end, start);
            start = Math.Clamp(start, 0, text.Length);
            end = Math.Clamp(end, 0, text.Length);

            newStart = start + open.Length;
            newEnd = end + open.Length;
            return text.Substring(0, start) + open + text.Substring(start, end - start) + close + text.Substring(end);
        }
    }
}
