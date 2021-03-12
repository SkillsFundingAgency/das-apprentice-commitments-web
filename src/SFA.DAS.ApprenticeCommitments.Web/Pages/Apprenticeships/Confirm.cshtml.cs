using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [Authorize]
    public class ConfirmApprenticeshipModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ApprenticeshipId { get; set; }

        public void OnGet()
        {
        }
    }
}