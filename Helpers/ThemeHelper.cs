using System;
using System.Collections.Generic;

namespace SchoolManagementSystem.Helpers
{
    public static class ThemeHelper
    {
        private static readonly (string Keyword, string Color)[] DesignationRules =
        {
            ("principal", "#DAA520"),    // Gold
            ("vice principal", "#C0C0C0"), // Silver
            ("teacher", "#1B4D8C"),      // Primary
            ("accountant", "#2E8B57"),   // Green
            ("librarian", "#800080"),    // Purple
            ("office staff", "#808080"), // Gray
            ("assistant head", "#00008B"), // Dark blue
            ("senior teacher", "#1E90FF"), // Dodger blue
            ("lab assistant", "#FF8C00"),  // Orange
            ("driver", "#8B4513"),        // Saddle brown
            ("guard", "#282828"),         // Dark gray
        };

        public static string GetThemeColor(string designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
                return "#1B4D8C";

            var des = designation.Trim().ToLowerInvariant();

            foreach (var (keyword, color) in DesignationRules)
            {
                if (des.Contains(keyword))
                    return color;
            }

            return "#1B4D8C";
        }
    }
}
