using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [HideNavigationBar]
    public class CheckYourDetails : PageModel
    {
        private readonly ApprenticeApi _apprentices;

        public CheckYourDetails(ApprenticeApi _apprentices)
        {
            this._apprentices = _apprentices;
        }

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateTime DateOfBirth { get; private set; }

        public async Task<ActionResult> OnGet(
            [FromServices] AuthenticatedUser user)
        {
            var apprentice = await _apprentices.TryGetApprentice(user.ApprenticeId);
            if (apprentice == null) return RedirectToPage("Account");

            FirstName = apprentice.FirstName;
            LastName = apprentice.LastName;
            DateOfBirth = apprentice.DateOfBirth;

            return Page();
        }
    }
}
