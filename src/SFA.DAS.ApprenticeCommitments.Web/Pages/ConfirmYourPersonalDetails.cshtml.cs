using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestEase;
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
    public class ConfirmYourPersonalDetailsModel : PageModel
    {
        private readonly RegistrationsService _registrations;
        private readonly IOuterApiClient _api;

        public ConfirmYourPersonalDetailsModel(RegistrationsService rapi, IOuterApiClient api)
        {
            _registrations = rapi;
            _api = api;
        }

        [BindProperty]
        [HiddenInput]
        public string EmailAddress { get; set; } = null!;

        [BindProperty]
        public string FirstName { get; set; } = null!;

        [BindProperty]
        public string LastName { get; set; } = null!;

        [BindProperty]
        public DateModel DateOfBirth { get; set; } = null!;

        public string FormHandler = "";

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public string? RegistrationCode { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            ViewData[ApprenticePortal.SharedUi.ViewDataKeys.MenuWelcomeText] = "Welcome";

            var apprentice = await _api.GetApprentice(user.ApprenticeId);

            return Page();
        }

        public async Task<IActionResult> OnGetRegister(
            [FromServices] AuthenticatedUser user)
        {
            FormHandler = "Register";

            var apprentice = await TryGetApprentice(user.ApprenticeId);

            EmailAddress = User.Identity.Name ?? "";
            FirstName = apprentice?.FirstName ?? "";
            LastName = apprentice?.LastName ?? "";
            //DateOfBirth.Date = apprentice?.DateOfBirth;

            if (apprentice == null)
            {
                await _registrations.FirstSeenOn(user.ApprenticeId, DateTime.UtcNow);
            }

            return Page();
        }

        private async Task<Apprentice?> TryGetApprentice(Guid apprenticeId)
        {
            try
            {
                return await _api.GetApprentice(apprenticeId);
            }
            catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser user, [FromServices] NavigationUrlHelper urlHelper)
        {
            await UpdateApprentice(user);

            if (ModelState.IsValid)
                return Redirect(urlHelper.Generate(NavigationSection.Home));
            else
                return Page();
        }

        private async Task UpdateApprentice(AuthenticatedUser user)
        {
            try
            {
                await _api.UpdateApprenticeAccount(new Apprentice
                {
                    Id = user.ApprenticeId,
                    FirstName = FirstName,
                    LastName = LastName,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    Email = EmailAddress,
                });
            }
            catch (DomainValidationException exception)
            {
                AddErrors(exception);
            }
        }

        public async Task<IActionResult> OnPostRegister([FromServices] AuthenticatedUser user)
        {
            await UpdateApprentice(user);

            if (ModelState.IsValid)
                return RedirectToAction("Register", "Registration", new { RegistrationCode });
            else
                return Page();
        }

        private void AddErrors(DomainValidationException exception)
        {
            ModelState.ClearValidationState(nameof(DateOfBirth));
            ModelState.ClearValidationState(nameof(LastName));
            ModelState.ClearValidationState(nameof(FirstName));

            foreach (var e in exception.Errors)
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