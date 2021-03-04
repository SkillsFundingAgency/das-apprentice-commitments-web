using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [Authorize]
    public class OverviewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long? ApprenticeshipId { get; set; }

        public IActionResult OnGet()
        {
            if (ApprenticeshipId == null) return Redirect("/Overview/1234");

            return Page();
        }
    }
}