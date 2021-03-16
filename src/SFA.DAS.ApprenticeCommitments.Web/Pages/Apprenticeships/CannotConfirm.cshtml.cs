using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class CannotConfirmApprenticeshipModel : PageModel, IBacklink
    {
        [BindProperty(SupportsGet = true)]
        public string ApprenticeshipId { get; set; }
        public string Backlink => $"/apprenticeships/{ApprenticeshipId}";

    }
}
