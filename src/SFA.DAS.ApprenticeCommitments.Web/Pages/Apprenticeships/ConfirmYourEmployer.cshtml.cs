using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ConfirmYourEmployerModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? ApprenticeshipId { get; set; }

        public void OnGet()
        {
        }
    }
}
