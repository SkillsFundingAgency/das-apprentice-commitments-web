using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class ConfirmationSectionTests
    {
        [Test, TestCustomisation]
        public async Task Sets_url_from_UrlHelper(string url, [Frozen] Mock<ISimpleUrlHelper> urlFactory, ConfirmationSectionTagHelper sut)
        {
            urlFactory.Setup(x =>
                x.Page(It.IsAny<ViewContext>(), "EmployerConfirmation", It.IsAny<object>()))
                .Returns(url);

            sut.AspPage = "EmployerConfirmation";
            await sut.ProcessAsync(_tagHelperContext, _tagHelperOutput);

            var result = _tagHelperOutput.Content.GetContent();
            result.Should().Contain($@"<a href=""{url}""");
        }

        [Test, TestCustomisation]
        public async Task Sets_complete_state_from_model(ConfirmationSectionTagHelper sut)
        {
            sut.Model.EmployerConfirmation = true;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(_tagHelperContext, _tagHelperOutput);

            var result = _tagHelperOutput.Content.GetContent();
            result.Should().Contain("background-color: red");
        }

        [Test, TestCustomisation]
        public async Task Sets_incorrect_state_from_model(ConfirmationSectionTagHelper sut)
        {
            sut.Model.EmployerConfirmation = false;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(_tagHelperContext, _tagHelperOutput);

            var result = _tagHelperOutput.Content.GetContent();
            result.Should().Contain("background-color: blue");
        }

        [Test, TestCustomisation]
        public async Task Sets_incomplete_state_from_model(ConfirmationSectionTagHelper sut)
        {
            sut.Model.EmployerConfirmation = null;

            sut.AspPage = "ConfirmYourEmployer";
            await sut.ProcessAsync(_tagHelperContext, _tagHelperOutput);

            var result = _tagHelperOutput.Content.GetContent();
            result.Should().Contain("background-color: green");
        }

        private readonly TagHelperContext _tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString());

        private readonly TagHelperOutput _tagHelperOutput = new TagHelperOutput("list",
                new TagHelperAttributeList(),
                (_, __) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

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
                fixture.Inject(AuthenticatedUser.FakeUser);
                fixture.Customize(new AutoMoqCustomization());
                return fixture;
            }
        }
    }
}