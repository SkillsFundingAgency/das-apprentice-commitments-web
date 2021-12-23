using AutoFixture;
using AutoFixture.NUnit3;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations
{
    public abstract class AutoDataCustomisationAttributeBase : AutoDataAttribute
    {
        protected AutoDataCustomisationAttributeBase(params ICustomization[] customisations)
            : base(() => CreateFixture(customisations))
        {
        }

        private static IFixture CreateFixture(ICustomization[] customisations)
        {
            var fixture = new Fixture();
            foreach (var c in customisations)
                fixture.Customize(c);
            return fixture;
        }
    }
}