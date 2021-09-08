using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestEase;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class AccountCreatedClaimTests
    {
        [Test, MoqAutoData]
        public async Task Users_without_an_apprentice_id_do_not_have_AccountCreated_claim(AuthenticationEvents sut, ClaimsPrincipal identity)
        {
            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_with_an_invalid_apprentice_id_do_not_have_AccountCreated_claim(AuthenticationEvents sut, ClaimsPrincipal identity, string notAGuid)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(notAGuid));

            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_not_created_an_account_do_not_have_AccountCreated_claim([Frozen] IOuterApiClient api, AuthenticationEvents sut, ClaimsPrincipal identity)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(Guid.NewGuid()));
            Mock.Get(api)
                .Setup(x => x.GetApprentice(It.IsAny<Guid>()))
                .Throws(NotFoundApiException);

            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_created_an_account_have_AccountCreated_claim_which_is_true([Frozen] IOuterApiClient api, AuthenticationEvents sut, Guid apprenticeId, ClaimsPrincipal identity, Apprentice apprentice)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(apprenticeId));
            Mock.Get(api)
                .Setup(x => x.GetApprentice(apprenticeId))
                .ReturnsAsync(apprentice);

            await sut.AddClaims(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "AccountCreated",
                Value = "True",
            });
            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "given_name",
                Value = apprentice.FirstName,
            });
            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "family_name",
                Value = apprentice.LastName,
            });
        }

        private static ClaimsIdentity ApprenticeIdClaimsIdentity(Guid apprenticeId)
            => ApprenticeIdClaimsIdentity(apprenticeId.ToString());

        private static ClaimsIdentity ApprenticeIdClaimsIdentity(string apprenticeId)
            => new ClaimsIdentity(new[] { new Claim("apprentice_id", apprenticeId) });

        private static ApiException NotFoundApiException =>
            new ApiException(new HttpRequestMessage(), new HttpResponseMessage(HttpStatusCode.NotFound), null);
    }
}