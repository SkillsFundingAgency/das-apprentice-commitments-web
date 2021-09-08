using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [HideNavigationBar]
    public class AccountModel : PageModel
    {
        private readonly ApprenticeApi _apprentices;
        private readonly NavigationUrlHelper _urlHelper;

        public AccountModel(ApprenticeApi api, NavigationUrlHelper urlHelper)
        {
            _apprentices = api;
            _urlHelper = urlHelper;
        }

        public string EmailAddress { get; set; } = null!;

        [BindProperty(Name = "mode", SupportsGet = true)]
        public string Mode { get; set; } = "";

        [BindProperty]
        public string FirstName { get; set; } = null!;

        [BindProperty]
        public string LastName { get; set; } = null!;

        [BindProperty]
        public DateModel DateOfBirth { get; set; } = null!;

        public string FormHandler { get; private set; } = "";

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            var apprentice = await _apprentices.TryGetApprentice(user.ApprenticeId);

            if (apprentice == null)
            {
                FormHandler = "Register";
            }
            else
            {
                ViewData[ApprenticePortal.SharedUi.ViewDataKeys.MenuWelcomeText] = "Welcome";
                FirstName = apprentice.FirstName;
                LastName = apprentice.LastName;
                DateOfBirth = new DateModel(apprentice.DateOfBirth);
            }

            return Page();
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser user)
        {
            try
            {
                await _apprentices.UpdateApprentice(user.ApprenticeId, FirstName, LastName, DateOfBirth.Date);

                return RedirectAfterUpdate();
            }
            catch (DomainValidationException exception)
            {
                AddErrors(exception);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegister([FromServices] AuthenticatedUser user)
        {
            FormHandler = "Register";

            try
            {
                await _apprentices.CreateApprentice(new Apprentice
                {
                    ApprenticeId = user.ApprenticeId,
                    FirstName = FirstName,
                    LastName = LastName,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    Email = user.Email.ToString(),
                });

                await HttpContext.UserAccountCreated();

                return RedirectAfterUpdate();
            }
            catch (DomainValidationException exception)
            {
                AddErrors(exception);
                return Page();
            }
        }

        private IActionResult RedirectAfterUpdate()
        {
            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return RedirectToAction("Register", "Registration");
            else
                return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));
        }

        private void AddErrors(DomainValidationException exception)
        {
            ModelState.ClearValidationState(nameof(DateOfBirth));
            ModelState.ClearValidationState(nameof(LastName));
            ModelState.ClearValidationState(nameof(FirstName));

            foreach (var e in exception.Errors.Distinct(new ErrorItemComparePropertyName()))
            {
                var (p, m) = e.PropertyName switch
                {
                    nameof(FirstName) => (e.PropertyName, "Enter your first name"),
                    nameof(LastName) => (e.PropertyName, "Enter your last name"),
                    nameof(DateOfBirth) => (e.PropertyName, "Enter your date of birth"),
                    "PersonalDetails" => ("PersonalDetails", "Details do not match any registered apprenticeship on our service. You can:"),
                    _ => ("", "Something went wrong")
                };

                if (p?.Length == 0 && ModelState.Keys.Contains("")) continue;

                if (p == "PersonalDetails")
                {
                    ModelState.AddModelError(p, "try again with the correct details");
                    ModelState.AddModelError(p, "contact your employer or training provider to fix your details");
                }
                else
                {
                    ModelState.AddModelError(p, m);
                }
            }
        }
    }
}