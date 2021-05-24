using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class HelpAndSupportModel : PageModel, IHasBackLink
    {        
        public string Backlink => $"/apprenticeships";
    }
}
