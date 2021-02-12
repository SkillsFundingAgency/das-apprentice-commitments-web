using System;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class RegistrationUser
    {
        public RegistrationUser(ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == "registration_id")
                ?? throw new Exception("There is no `registration_id` claim.");

            if(!Guid.TryParse(claim.Value, out var registrationId))
                throw new Exception($"`{claim.Value}` in claim `registration_id` is not a valid identifier");

            RegistrationId = registrationId;
        }

        public Guid RegistrationId { get; }
    }
}