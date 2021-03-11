using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? ApprenticeshipId { get; set; }

        public void OnGet()
        {
        }
    }
}
