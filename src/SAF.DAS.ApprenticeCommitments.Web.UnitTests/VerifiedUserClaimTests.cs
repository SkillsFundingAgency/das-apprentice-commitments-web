using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class VerifiedUserClaimTests
    {
        [Test, MoqAutoData]
        public async Task Users_without_a_registration_id_do_not_have_VerifiedUser_claim(AuthenticationEvents sut, ClaimsPrincipal identity)
        {
            await sut.AddUserVerifiedClaim(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("VerifiedUser", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_with_an_invalid_registration_id_do_not_have_VerifiedUser_claim(AuthenticationEvents sut, ClaimsPrincipal identity, string notAGuid)
        {
            identity.AddIdentity(new ClaimsIdentity(new[] { new Claim("registration_id", notAGuid) }));

            await sut.AddUserVerifiedClaim(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("VerifiedUser", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_do_not_exist_do_not_have_VerifiedUser_claim([Frozen] IOuterApiClient api, AuthenticationEvents sut, ClaimsPrincipal identity, Guid registrationId)
        {
            identity.AddIdentity(RegistrationIdClaimsIdentity(registrationId.ToString()));
            Mock.Get(api)
                .Setup(x => x.GetRegistration(It.IsAny<Guid>()))
                .Throws(new Exception("Registration not found"));

            await sut.AddUserVerifiedClaim(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("VerifiedUser", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_not_completed_verification_do_not_have_VerifiedUser_claim([Frozen] IOuterApiClient api, AuthenticationEvents sut, Guid registrationId, ClaimsPrincipal identity)
        {
            identity.AddIdentity(RegistrationIdClaimsIdentity(registrationId.ToString()));
            Mock.Get(api)
                .Setup(x => x.GetRegistration(registrationId))
                .ReturnsAsync(new VerifyRegistrationResponse { HasCompletedVerification = false });

            await sut.AddUserVerifiedClaim(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("VerifiedUser", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_completed_verification_have_VerifiedUser_claim_which_is_true([Frozen] IOuterApiClient api, AuthenticationEvents sut, Guid registrationId, ClaimsPrincipal identity)
        {
            identity.AddIdentity(RegistrationIdClaimsIdentity(registrationId.ToString()));
            Mock.Get(api)
                .Setup(x => x.GetRegistration(registrationId))
                .ReturnsAsync(new VerifyRegistrationResponse { HasCompletedVerification = true });

            await sut.AddUserVerifiedClaim(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "VerifiedUser",
                Value = "True"
            });
        }

        private static ClaimsIdentity RegistrationIdClaimsIdentity(string notAGuid)
        {
            return new ClaimsIdentity(new[] { new Claim("registration_id", notAGuid) });
        }
    }
}