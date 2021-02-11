using AngleSharp.Html.Dom;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "ConfirmIdentity")]
    public class ConfirmIdentitySteps : StepsBase
    {
        private readonly TestContext _context;
        private Guid _registrationId = Guid.NewGuid();
        private ConfirmYourIdentityModel _postedRegistration;

        public ConfirmIdentitySteps(TestContext context) : base(context)
        {
            _context = context;
        }

        [Given("the apprentice has not logged in")]
        public void GivenTheApprenticeHasNotLoggedIn()
        {
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_registrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_registrationId.ToString());
        }

        [Given("the apprentice has not verified their identity")]
        public void GivenTheApprenticeHasNotVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/registrations/{_registrationId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _registrationId,
                        EmailAddress = "bob",
                    }));
        }

        [When(@"accessing the ""(.*)"" page")]
        public async Task WhenAccessingThePage(string page)
        {
            await _context.Web.Get(page);
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the verify identity page")]
        public void ThenTheApprenticeShouldSeeTheVerifyIdentityPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourIdentityModel>().Which.EmailAddress.Should().Be("bob");
        }

        [Given("the apprentice has verified their identity")]
        public void GivenTheApprenticeHasVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/registrations/{_registrationId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _registrationId,
                        EmailAddress = "bob",
                        UserId = 12,
                    }));
        }

        [When(@"the apprentice should be shown the ""(.*)"" page")]
        public void WhenTheApprenticeShouldBeShownThePage(string page)
        {
            _context.ActionResult.LastPageResult.Should().NotBeNull();
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<OverviewModel>();
        }

        [When(@"the apprentice verifies their identity with")]
        public async Task GivenTheApprenticeVerifiesTheirIdentityWith(Table table)
        {
            _context.OuterApi.MockServer
                .Given(Request.Create().WithPath("/registrations*"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

            _postedRegistration = table.CreateInstance(() => new ConfirmYourIdentityModel(null));

            var get = await _context.Web.Get("ConfirmYourIdentity");
            using var content = await HtmlHelpers.GetDocumentAsync(get);

            var form = (IHtmlFormElement)content.QuerySelector("form");
            (form["FirstName"] as IHtmlInputElement).Value = _postedRegistration.FirstName;
            (form["LastName"] as IHtmlInputElement).Value = _postedRegistration.LastName;
            (form["NationalInsuranceNumber"] as IHtmlInputElement).Value = _postedRegistration.NationalInsuranceNumber;

            var button = (IHtmlButtonElement)content.QuerySelector("button");
            var formSubmission = form.GetSubmission(button);

            var target = (Uri)formSubmission.Target;

            var submitted = new StreamReader(formSubmission.Body).ReadToEnd();

            var request = new HttpRequestMessage(new HttpMethod(formSubmission.Method.ToString()), target)
            {
                Content = new StreamContent(formSubmission.Body)
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            await _context.Web.Client.SendAsync(request);
        }

        [Then("verification is successfull")]
        public void ThenTheVerificationIsSuccessfull()
        {
            ThenTheResponseStatusCodeShouldBeOk();
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/registrations*")
                    .UsingPost()
                                                                               );

            registrationPosts.Should().NotBeEmpty();

            var post = registrationPosts.First();

            post.RequestMessage.Path.Should().Be($"/registrations/{_registrationId}");
            var reg = JsonConvert.DeserializeObject<VerifyRegistrationCommand>(post.RequestMessage.Body);
            reg.Should().BeEquivalentTo(new VerifyRegistrationCommand
            {
                RegistrationId = _registrationId,
                FirstName = _postedRegistration.FirstName,
                LastName = _postedRegistration.LastName,
                NationalInsuranceNumber = _postedRegistration.NationalInsuranceNumber,
            });
        }
    }
}