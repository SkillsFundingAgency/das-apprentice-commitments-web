using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations
{
    public class PageAutoDataAttribute : AutoDataAttribute
    {
        public PageAutoDataAttribute() : base(() => CreateFixture())
        { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Register((ClaimsPrincipal claims) =>
                new DefaultHttpContext { User = claims });
            fixture.Register((DefaultHttpContext http, RouteData route) =>
                new ActionContext(http, route, new PageActionDescriptor()));
            fixture.Register((ActionContext a) => new PageContext(a)
            {
                ViewData = fixture.Create<ViewDataDictionary>()
            });
            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            return fixture;
        }
    }
}