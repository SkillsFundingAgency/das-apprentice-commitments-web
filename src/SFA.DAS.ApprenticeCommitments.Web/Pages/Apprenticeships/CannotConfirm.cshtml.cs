using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class CannotConfirmApprenticeshipModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? ApprenticeshipId { get; set; }
    }
}
