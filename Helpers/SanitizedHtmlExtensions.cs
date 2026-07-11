using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Helpers
{
    public static class SanitizedHtmlExtensions
    {
        public static HtmlString SanitizedHtml(this IHtmlHelper helper, string? html)
        {
            return new HtmlString(HtmlSanitizerHelper.SanitizeHtml(html));
        }
    }
}
