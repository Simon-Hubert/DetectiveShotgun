using System;
using System.Globalization;
using System.Linq;

namespace DShotgun.RichText
{
    /// <summary>
    /// Lecture / Parsing propre
    /// </summary>
    public static class TagValues
    {
        private static readonly string[] COLOR_NAMES = { "red", "lightblue", "blue", "grey", "black", "green", "white", "orange", "purple", "yellow" };

        public static bool TryParseDecimal(string value, out float number) {
            number = 0f;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return float.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out number)
                   && !float.IsNaN(number) && !float.IsInfinity(number);
        }

        public static bool TryParsePositive(string value, out float number) {
            return TryParseDecimal(value, out number) && number > 0f;
        }

        // « 80% » : TMP gère les pourcentages de <size>, pas de <cspace>
        public static bool TryParsePercent(string value, out float percent) {
            percent = 0f;
            if (string.IsNullOrEmpty(value) || !value.EndsWith("%")) return false;
            return TryParsePositive(value.Substring(0, value.Length - 1), out percent);
        }

        /// <summary>
        /// Normalise une valeur de longueur au cas ou 
        /// </summary>
        /// <param name="value">La valeur à normaliser.</param>
        /// <param name="normalized">La valeur normalisée.</param>
        /// <returns>Renvoie la valeur normalisée.</returns>
        public static bool TryNormalizeLength(string value, out string normalized) {
            normalized = null;
            if (string.IsNullOrWhiteSpace(value)) return false;

            string unit = string.Empty;
            string number = value.Trim();
            if (number.EndsWith("px", StringComparison.OrdinalIgnoreCase)) unit = "px";
            else if (number.EndsWith("em", StringComparison.OrdinalIgnoreCase)) unit = "em";
            if (unit.Length > 0) number = number.Substring(0, number.Length - unit.Length);

            if (!TryParseDecimal(number, out float parsed)) return false;
            normalized = FormatInvariant(parsed) + unit;
            return true;
        }

        /// <summary>
        /// #RGB, #RGBA, #RRGGBB, #RRGGBBAA ou un nom de couleur de TMP.
        /// </summary>
        /// <param name="value">La valeur à normaliser.</param>
        /// <param name="normalized">La valeur normalisée.</param>
        /// <returns>Renvoie la valeur normalisée.</returns>
        public static bool TryNormalizeColor(string value, out string normalized) {
            normalized = null;
            if (string.IsNullOrEmpty(value)) return false;

            if (value[0] == '#')
            {
                string hex = value.Substring(1);
                if (!(hex.Length is 3 or 4 or 6 or 8) || !hex.All(Uri.IsHexDigit)) return false;
                if (hex.Length <= 4) hex = string.Concat(hex.Select(c => new string(c, 2)));
                normalized = "#" + hex.ToUpperInvariant();
                return true;
            }

            string name = value.ToLowerInvariant();
            if (!COLOR_NAMES.Contains(name)) return false;
            normalized = name;
            return true;
        }

        public static string ColorHelp => "#RGB, #RGBA, #RRGGBB, #RRGGBBAA ou un nom (" + string.Join(", ", COLOR_NAMES) + ")";
        
        public static string FormatInvariant(float number) => number.ToString(CultureInfo.InvariantCulture);
    }
}
