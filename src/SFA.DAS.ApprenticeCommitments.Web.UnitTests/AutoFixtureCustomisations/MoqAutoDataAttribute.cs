using AutoFixture.AutoMoq;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations
{
    public class MoqAutoDataAttribute : AutoDataCustomisationAttributeBase
    {
        public MoqAutoDataAttribute() : base(new AutoMoqCustomization { ConfigureMembers = false })
        {
        }
    }
}