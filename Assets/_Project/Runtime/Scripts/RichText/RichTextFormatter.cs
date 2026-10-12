using System.Text;
using UnityEngine;

namespace DShotgun
{
    public static class RichTextFormatter
    {
        // Espace de largeur nulle ajouté après chaque '<' saisi : il ne sera jamais lu comme une balise.
        private const string TAG_BREAKER = "​";

        public static string Format(RichTextData data) {
            string text = data?.Text;
            if (string.IsNullOrEmpty(text)) return string.Empty;

            int count = text.Length;
            bool[] bold = new bool[count];
            bool[] italic = new bool[count];
            string[] color = new string[count];

            // Style par caractère : gras et italique s'additionnent, la dernière couleur de la liste l'emporte.
            foreach (TextStyle style in data.Styles) {
                if (style == null) continue;
                int start = Mathf.Clamp(style.StartIndex, 0, count);
                int end = Mathf.Clamp(style.StartIndex + style.Length, 0, count);
                for (int i = start; i < end; i++) {
                    bold[i] |= style.IsBold;
                    italic[i] |= style.IsItalic;
                    if (!string.IsNullOrEmpty(style.ColorHex)) color[i] = style.ColorHex;
                }
            }

            // Un segment par suite de caractères de même style, chacun avec ses balises :
            // aucune imbrication cassée quand des plages se chevauchent.
            StringBuilder builder = new();
            int segmentStart = 0;
            for (int i = 1; i <= count; i++) {
                bool sameStyle = i < count
                    && bold[i] == bold[segmentStart]
                    && italic[i] == italic[segmentStart]
                    && color[i] == color[segmentStart];
                if (sameStyle) continue;

                AppendSegment(builder, text.Substring(segmentStart, i - segmentStart),
                    bold[segmentStart], italic[segmentStart], color[segmentStart]);
                segmentStart = i;
            }

            return builder.ToString();
        }

        private static void AppendSegment(StringBuilder builder, string segment, bool bold, bool italic, string color) {
            if (color != null) builder.Append("<color=").Append(color).Append('>');
            if (bold) builder.Append("<b>");
            if (italic) builder.Append("<i>");

            builder.Append(segment.Replace("<", "<" + TAG_BREAKER));

            if (italic) builder.Append("</i>");
            if (bold) builder.Append("</b>");
            if (color != null) builder.Append("</color>");
        }
    }
}
