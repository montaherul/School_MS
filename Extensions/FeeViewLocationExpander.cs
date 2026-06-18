using Microsoft.AspNetCore.Mvc.Razor;

namespace SchoolManagementSystem.Extensions
{
    public class FeeViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var feeLocations = new[]
            {
                "/Views/Fee/{1}/{0}.cshtml",
                "/Views/Fee/Shared/{0}.cshtml"
            };
            return feeLocations.Concat(viewLocations);
        }
    }
}
