using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class CannotConfirmApprenticeshipModel : PageModel, IHasBackLink
    {
        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty(Name = "Entity", SupportsGet = true)]
        public string Entity { get; set; } = "";

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";
    }
}