using System.Collections.Generic;
using System.Text;

namespace DShotgun.RichText
{
    /// <summary>
    /// Traduit les instructions du parser en texte final pour une cible (TMP, aperçu IMGUI, texte nu)
    /// </summary>
    public static class RichTextEmitter
    {
        /// <summary>
        /// Écrit le texte final et relève les caractères visibles couverts par chaque balise.
        /// </summary>
        /// <param name="instructions"> Instructions produites par le <see cref="RichTextParser"/></param>
        /// <param name="target"> Lecteur du texte final (TMP, aperçu IMGUI ou texte nu)</param>
        /// <param name="spans"> Liste remplie avec la plage de chaque balise, dans l'ordre des fermetures</param>
        /// <returns> Renvoie le texte prêt à être affiché par la cible</returns>
        public static string Emit(Queue<Instruction> instructions, RichTextTarget target, List<TagSpan> spans) {
            StringBuilder output = new();
            Stack<(Instruction push, int start)> openTags = new();
            int visibleLength = 0;   // caractères affichés, balises non comptées

            while (instructions.Count > 0) {
                Instruction instruction = instructions.Dequeue();
                switch (instruction.Type) {
                    case InstructionType.Text:
                        output.Append(Escape(instruction.Text, target));
                        visibleLength += instruction.Text.Length;   // longueur d'origine : l'échappement n'ajoute aucun caractère visible
                        break;
                    case InstructionType.Push:
                        output.Append(instruction.Tag.Open(target, instruction.Value));   // la balise décide comment elle s'écrit
                        openTags.Push((instruction, visibleLength));
                        break;
                    case InstructionType.Pop:
                        output.Append(instruction.Tag.Close(target));
                        (Instruction push, int start) = openTags.Pop();   // le parser garantit l'ordre
                        spans.Add(new TagSpan(push.Tag.Name, push.Value, start, visibleLength - start));
                        break;
                }
            }

            return output.ToString();
        }
        
        // Un « < » du texte ne doit pas être relu comme une balise par TMP ou l'IMGUI
        private static string Escape(string text, RichTextTarget target) {
            if (text.IndexOf('<') < 0) return text;
            return target switch
            {
                RichTextTarget.Tmp => text.Replace("<", "<noparse><</noparse>"),
                RichTextTarget.Preview => text.Replace("<", "<​"),
                _ => text
            };
        }
    }
    
    /// <summary>
    /// Plage de caractères visibles couverte par une balise (mêmes index que les caractères TMP)
    /// </summary>
    public readonly struct TagSpan
    {
        public string TagName { get; }
        public string Value { get; }
        public int Start { get; }
        public int Length { get; }

        public TagSpan(string tagName, string value, int start, int length) {
            TagName = tagName;
            Value = value;
            Start = start;
            Length = length;
        }

        public override string ToString() => $"{TagName}{(Value == null ? "" : "=" + Value)} [{Start}, {Start + Length}[";
    }
}
