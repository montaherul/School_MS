using System;
using System.Collections.Generic;

namespace SchoolManagementSystem.Helpers
{
    /// <summary>
    /// Helper methods for UI theming used in Razor views.
    /// </summary>
    public static class ThemeHelper
    {
        private static readonly Dictionary<string, string> DesignationColors = new()
        {
            { "Principal", "#DAA520" },       // Gold
            { "Vice Principal", "#C0C0C0" },   // Silver
            { "Teacher", "#1B4D8C" },          // Primary school color
            { "Accountant", "#2E8B57" },      // Green
            { "Librarian", "#800080" },       // Purple
            { "Office Staff", "#808080" },    // Gray
            // default fallback
        };

        /// <summary>
        /// Returns a CSS color string based on the employee designation.
        /// If the designation is not in the predefined list, the primary color is returned.
        /// </summary>
        /// <param name="designation">Employee designation (e.g., Teacher, Principal)</param>
        /// <returns>Hex color string (including leading '#')</returns>
        public static string GetThemeColor(string designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
                return "#1B4D8C"; // primary default

            if (DesignationColors.TryGetValue(designation.Trim(), out var color))
                return color;

            // fallback to primary color
            return "#1B4D8C";
        }
    }
}
