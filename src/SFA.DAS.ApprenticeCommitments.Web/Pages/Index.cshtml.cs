using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class IndexModel : PageModel
    {
        public string Name { get; private set; }

        public void OnGet()
        {
            Name = "Get called";
        }
    }
}