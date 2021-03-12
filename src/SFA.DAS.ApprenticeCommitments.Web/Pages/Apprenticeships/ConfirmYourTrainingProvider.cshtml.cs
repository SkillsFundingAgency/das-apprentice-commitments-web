using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        public string TrainingProviderName { get; private set; }

        public void OnGet()
        {
            TrainingProviderName = "the name of the training provider goes here";
        }
    }
}
