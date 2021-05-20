using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class IndexModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Until some actual functionality is implemented just redirect to the ConfirmYourIdentity page
            // Note: the SignOut page redirects to Index.html from the 'Log back in' link
            return RedirectToPage("ConfirmYourIdentity");
        }
    }
}