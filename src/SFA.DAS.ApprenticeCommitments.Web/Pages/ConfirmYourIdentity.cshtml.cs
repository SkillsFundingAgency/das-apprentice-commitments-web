using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ConfirmYourIdentityModel : PageModel
    {
        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your national insurance number")]
        public string NationalInsuranceNumber{ get; set; }

        public void OnGet(
            string firstName,
            string lastName,
            string nationalInsuranceNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            NationalInsuranceNumber = nationalInsuranceNumber;
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/ConfirmYourIdentity", new
            {
                FirstName,
                LastName,
                NationalInsuranceNumber,
            });
        }
    }
}