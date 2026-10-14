using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DShotgun
{
    public static class RichTextFormatter
    {
        private const string TAG_BREAKER = "​";

        private static bool IsSameStyle(int i, int count, bool[] bold, bool[] italic, string[] color, int segmentStart)
        {
            if (i < count)
            {
                return bold[i] == bold[segmentStart]
                    && italic[i] == italic[segmentStart]
                    && color[i] == color[segmentStart];
            }

            return false;
        }

        /// <summary>
        /// Génère le texte avec les balises de style à partir des données de <see cref="RichTextData"/>
        /// </summary>
        /// <param name="data"> le texte stylisé</param>
        /// <returns> Renvoie le texte avec les balises insérées</returns>
        public static string GenerateTaggedText(RichTextData data) //generation du texte + balise
        {
            if (data == null) return string.Empty;
            string text = data.Text ?? string.Empty;
            int count = text.Length;
            if (count == 0) return string.Empty;

            bool[] bold = new bool[count];
            bool[] italic = new bool[count];
            string[] color = new string[count];

            foreach (TextStyle style in data.Styles)
            {
                if (style == null) continue;
                int start = Mathf.Clamp(style.StartIndex, 0, count);
                int end = Mathf.Clamp(style.StartIndex + style.Length, 0, count);
                for (int i = start; i < end; i++)
                {
                    bold[i] |= style.IsBold;
                    italic[i] |= style.IsItalic;
                    if (!string.IsNullOrEmpty(style.ColorHex)) color[i] = style.ColorHex;
                }
            }

            StringBuilder sb = new StringBuilder();
            int segStart = 0;
            for (int i = 1; i <= count; i++)
            {
                bool same = i < count && bold[i] == bold[segStart] && italic[i] == italic[segStart] && color[i] == color[segStart];
                if (same) continue;
                
                if (!string.IsNullOrEmpty(color[segStart])) sb.Append("<color=").Append(color[segStart]).Append('>');
                if (bold[segStart]) sb.Append("<b>");
                if (italic[segStart]) sb.Append("<i>");

                sb.Append(text, segStart, i - segStart);   // pas d'échappement : en v2, un « < » seul reste du texte

                if (italic[segStart]) sb.Append("</i>");
                if (bold[segStart]) sb.Append("</b>");
                if (!string.IsNullOrEmpty(color[segStart])) sb.Append("</color>");

                segStart = i;
            }

            return sb.ToString();
        }


        public static bool TryParseTaggedText(string input, out string plainText, out System.Collections.Generic.List<TextStyle> styles) //parse les balises
        {
            plainText = string.Empty;
            styles = new List<TextStyle>();
            if (string.IsNullOrEmpty(input)) return false;

            StringBuilder sb = new StringBuilder();
            var boldList = new List<bool>();
            var italicList = new List<bool>();
            var colorList = new List<string>();

            bool curBold = false;
            bool curItalic = false;
            string curColor = null;

            int i = 0;
            while (i < input.Length)
            {
                char c = input[i];
                if (c == '<')
                {
                    int j = input.IndexOf('>', i + 1);
                    if (j == -1)
                    {
                        // malformed, treat '<' as literal
                        sb.Append(c);
                        boldList.Add(curBold);
                        italicList.Add(curItalic);
                        colorList.Add(curColor);
                        i++;
                        continue;
                    }

                    string tag = input.Substring(i + 1, j - i - 1).Trim();
                    string tagLower = tag.ToLowerInvariant();
                    
                    if (tagLower == "b") { curBold = true; }
                    else if (tagLower == "i") { curItalic = true; }
                    else if (tagLower.StartsWith("color="))
                    {
                        int eq = tag.IndexOf('=');
                        if (eq >= 0 && eq + 1 < tag.Length)
                        {
                            string value = tag.Substring(eq + 1);
                            curColor = value;
                        }
                    }
                    else if (tagLower == "/b") { curBold = false; }
                    else if (tagLower == "/i") { curItalic = false; }
                    else if (tagLower == "/color") { curColor = null; }
                    else
                    {
                        sb.Append('<');
                        for (int k = i + 1; k <= j; k++) sb.Append(input[k]);
                        for (int k = i + 1; k <= j; k++) { boldList.Add(curBold); italicList.Add(curItalic); colorList.Add(curColor); }
                        i = j + 1;
                        continue;
                    }

                    i = j + 1;
                }
                else
                {
                    sb.Append(c);
                    boldList.Add(curBold);
                    italicList.Add(curItalic);
                    colorList.Add(curColor);
                    i++;
                }
            }

            int count = sb.Length;
            if (count == 0)
            {
                plainText = string.Empty;
                return true;
            }

            int segStart = 0;
            for (int idx = 1; idx <= count; idx++)
            {
                bool same = IsSameStyle(idx, count - 1, boldList.ToArray(), italicList.ToArray(), colorList.ToArray(), segStart);
                if (same) continue;

                Debug.Log($"{segStart} size {boldList.Count}");
                bool segBold = boldList[segStart];
                bool segItalic = italicList[segStart];
                string segColor = colorList[segStart];
                int segLen = idx - segStart;
                if (segBold || segItalic || !string.IsNullOrEmpty(segColor))
                {
                    styles.Add(new TextStyle(segStart, segLen, segBold, segItalic, segColor ?? ""));
                }
                segStart = idx;
            }

            plainText = sb.ToString();
            return true;
        }
        
        public static void MapDisplayRangeToPlainRange(string display, int displayStart, int displayLength, out int plainStart, out int plainLength)
        {
            plainStart = 0;
            int count = display?.Length ?? 0;
            if (displayStart < 0) displayStart = 0;
            if (displayLength < 0) displayLength = 0;

            int ScanPlainCountUpTo(int limit)
            {
                int p = 0;
                for (int i = 0; i < limit && i < count; i++)
                {
                    char c = display[i];
                    if (c == '<')
                    {
                        if (i + 1 < count && display[i + 1] == TAG_BREAKER[0])
                        {
                            p++; 
                            i++; 
                            continue;
                        }
                        int j = display.IndexOf('>', i + 1);
                        if (j == -1)
                        {
                            p++;
                            continue;
                        }
                        i = j;
                        continue;
                    }
                    else
                    {
                        p++;
                    }
                }
                return p;
            }

            int plainBeforeStart = ScanPlainCountUpTo(displayStart);
            int plainBeforeEnd = ScanPlainCountUpTo(displayStart + displayLength);
            plainStart = plainBeforeStart;
            plainLength = Mathf.Max(0, plainBeforeEnd - plainBeforeStart);
        }
    }
}
