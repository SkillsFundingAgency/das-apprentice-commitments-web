using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using SFA.DAS.ApprenticeCommitments.Web.Pages;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{

    public class MoqAutoDataAttribute : AutoDataAttribute
    {
        public MoqAutoDataAttribute() : base(() => CreateFixture())
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