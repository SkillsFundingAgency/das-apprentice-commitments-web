using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingDetailsModel : PageModel, IHasBackLink
    {
        [BindProperty(SupportsGet = true)]
        public string? ApprenticeshipId { get; set; }
        public string Backlink => $"/apprenticeships/{ApprenticeshipId}";

        public void OnGet()
        {
        }
    }
}
