using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class AccountModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "Confirm your identity";
        }
    }
}
