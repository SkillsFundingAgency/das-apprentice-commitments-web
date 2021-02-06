using AutoFixture.AutoMoq;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations
{
    public class MoqAutoDataAttribute : AutoDataCustomisationAttributeBase
    {
        public MoqAutoDataAttribute() : base(new AutoMoqCustomization())
        {
        }
    }

}