using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class ConfirmationSectionTests : TagHelperFixture
    {
        [Test, TestCustomisation]
        public async Task Sets_url_from_UrlHelper(string url, [Frozen] Mock<ISimpleUrlHelper> urlFactory, ConfirmationSectionTagHelper sut)
        {
            urlFactory.Setup(x =>
                x.Page(It.IsAny<ViewContext>(), "EmployerConfirmation", It.IsAny<object>()))
                .Returns(url);

            sut.AspPage = "EmployerConfirmation";
            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            var result = TagHelperOutput.Content.GetContent();
            result.Should().Contain($@"<a href=""{url}""");
        }

        [Test, TestCustomisation]
        public async Task Sets_complete_state_from_confirmation_status(ConfirmationSectionTagHelper sut)
        {
            sut.ConfirmationStatus = true;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            var result = TagHelperOutput.Content.GetContent();
            result.Should().Contain("govuk-tag--green");
            result.Should().Contain("Complete");
        }

        [Test, TestCustomisation]
        public async Task Sets_incorrect_state_from_model(ConfirmationSectionTagHelper sut)
        {
            sut.ConfirmationStatus = false;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            var result = TagHelperOutput.Content.GetContent();
            result.Should().Contain("govuk-tag--red");
            result.Should().Contain("Waiting for<br/>correction");
        }

        [Test, TestCustomisation]
        public async Task Sets_incomplete_state_from_model(ConfirmationSectionTagHelper sut)
        {
            sut.ConfirmationStatus = null;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            var result = TagHelperOutput.Content.GetContent();
            result.Should().Contain("govuk-tag--yellow");
            result.Should().Contain("Incomplete");
        }

        private class TestCustomisationAttribute : AutoDataAttribute
        {
            public TestCustomisationAttribute() : base(() => CreateFixture())
            { }

            private static IFixture CreateFixture()
            {
                var fixture = new Fixture();
                fixture.Register(() => new ViewContext());
                fixture.Register((DefaultHttpContext http, RouteData route) =>
                    new ActionContext(http, route, new PageActionDescriptor()));
                fixture.Register((ActionContext a) => new PageContext(a));
                fixture.Inject(TestHelpers.FakeLocalUserFullyVerified);
                fixture.Customize(new AutoMoqCustomization());
                return fixture;
            }
        }
    }
}