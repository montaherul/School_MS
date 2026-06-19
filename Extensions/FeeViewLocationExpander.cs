using Microsoft.AspNetCore.Mvc.Razor;

namespace SchoolManagementSystem.Extensions
{
    public class FeeViewLocationExpander : IViewLocationExpander
    {
        private static readonly HashSet<string> FeeControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "FeeDashboard", "FeeCategory", "FeeStructure", "StudentFeeAssignment",
            "FeeInvoice", "FeeInvoiceItem", "FeePayment", "FeeDiscount",
            "FeeWaiver", "FeeRefund", "FeeLedger", "FeeCollectionSummary",
            "LateFeeRule", "FineRule", "FeeReport", "Payment"
        };

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (FeeControllers.Contains(context.ControllerName))
            {
                var feeLocations = new[]
                {
                    "/Views/Fee/{1}/{0}.cshtml",
                    "/Views/Fee/Shared/{0}.cshtml"
                };
                return feeLocations.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
