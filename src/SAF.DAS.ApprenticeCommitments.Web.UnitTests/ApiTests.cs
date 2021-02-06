using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class ApiTests
    {
        [Test, MoqAutoData]
        public async Task Can_retrieve_registration_details(
            [Frozen] Mock<ApiClient> client,
            RegistrationsService sut,
            Registration registration)
        {
            client.Setup(x => x.GetRegistration(registration.Id)).Returns(Task.FromResult(registration));
            (await sut.GetRegistration(registration.Id)).Should().BeEquivalentTo(registration);
        }
    }
}