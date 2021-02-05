using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System.Linq;
using System.Security.Claims;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations
{
    public class PageAutoDataAttribute : AutoDataAttribute
    {
        public PageAutoDataAttribute() : base(() => CreateFixture())
        { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<ConfirmYourIdentityModel>(c => c.Without(m => m.PageContext));
            fixture.Customize(new AutoMoqCustomization());
            return fixture;
        }
    }
}