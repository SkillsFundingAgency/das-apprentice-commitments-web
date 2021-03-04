using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [Authorize]
    public class OverviewModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
