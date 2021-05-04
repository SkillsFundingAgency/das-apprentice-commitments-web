using FluentAssertions;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System;
using System.Security.Claims;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class ApprenticeIdFromRegisatrionIdTests
    {
        [Test, MoqAutoData]
        public void Missing_both_claims_is_ignored(AuthenticationEvents sut, ClaimsPrincipal identity, Guid id)
        {
            sut.ConvertRegistrationIdToApprenticeId(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("registration_id", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("apprentice_id", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public void Registration_claim_is_converted_to_apprentic_claim(AuthenticationEvents sut, ClaimsPrincipal identity, Guid id)
        {
            identity.AddIdentity(new ClaimsIdentity(new[] { new Claim("registration_id", id.ToString()) }));

            sut.ConvertRegistrationIdToApprenticeId(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "apprentice_id",
                Value = id.ToString(),
            });
        }

        [Test, MoqAutoData]
        public void Registration_claim_does_not_overwrite_existing_apprentice_claim(AuthenticationEvents sut, ClaimsPrincipal identity, Guid registrationId, Guid apprenticeId)
        {
            identity.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("registration_id", registrationId.ToString()),
                new Claim("apprentice_id", apprenticeId.ToString()),
            }));

            sut.ConvertRegistrationIdToApprenticeId(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "apprentice_id",
                Value = apprenticeId.ToString(),
            });

            identity.HasClaim("apprentice_id", registrationId.ToString()).Should().BeFalse();
        }
    }
}