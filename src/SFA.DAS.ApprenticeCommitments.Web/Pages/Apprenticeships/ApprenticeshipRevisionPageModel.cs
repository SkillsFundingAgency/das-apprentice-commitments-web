using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ApprenticeshipRevisionPageModel : PageModel, IHasBackLink
    {
        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public long RevisionId { get; set; }

        public DeliveryModel DeliveryModel { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";
    }
}