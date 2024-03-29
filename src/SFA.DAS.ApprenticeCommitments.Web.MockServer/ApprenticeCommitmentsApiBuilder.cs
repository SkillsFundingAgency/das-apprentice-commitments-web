using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Net;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public class ApprenticeCommitmentsApiBuilder
    {
        private static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        private readonly WireMockServer _server;

        public ApprenticeCommitmentsApiBuilder(int port)
        {
            _server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = port,
                UseSSL = true,
                StartAdminInterface = true,
                Logger = new WireMockConsoleLogger(),
            });
        }

        public static ApprenticeCommitmentsApiBuilder Create(int port)
        {
            return new ApprenticeCommitmentsApiBuilder(port);
        }

        public ApprenticeCommitmentsApi Build()
        {
            return new ApprenticeCommitmentsApi(_server);
        }

        public ApprenticeCommitmentsApiBuilder WithUsersFirstLogin()
        {
            var data = new VerifyRegistrationResponse
            {
                Email = "bob@example.com",
                ApprenticeId = Guid.NewGuid(),
                HasViewedVerification = true,
                HasCompletedVerification = true,
            };
            var response = JsonConvert.SerializeObject(data, DefaultSerializerSettings);
            
            _server.Given(
                Request.Create()
                    .WithPath("/apprentices/*")
                    .UsingPatch())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                Request.Create()
                    .WithPath("/apprentices")
                    .UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                Request.Create()
                    .WithPath("/apprenticeships")
                    .UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                Request.Create()
                    .WithPath("/registrations/*")
                    .UsingGet()
                         )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(response)
                            );

            _server.Given(
                Request.Create()
                    .WithPath("/registrations")
                    .UsingPost()
                    .WithBody(new JmesPathMatcher("contains(FirstName, 'Error')")))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.BadRequest)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(new[]
                        {
                            new { PropertyName = "FirstName", ErrorMessage = "Invalid FirstName" },
                            new { PropertyName = "LastName", ErrorMessage = "Invalid LastName" },
                            new { PropertyName = "DateOfBirth", ErrorMessage = "Invalid DateOfBirth" },
                            new { PropertyName = "NationalInsuranceNumber", ErrorMessage = "Invalid NationalInsuranceNumber" },
                            new { PropertyName = "Email", ErrorMessage = "Invalid email" },
                            new { PropertyName = "PersonalDetails", ErrorMessage = "DoB mismatch" },
                            new { PropertyName = (string)null, ErrorMessage = "Registration {Id} id already verified" },
                        }));

            _server.Given(
                Request.Create()
                    .WithPath("/registrations")
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                Request.Create()
                    .WithPath("/apprentices")
                    .UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                Request.Create()
                    .WithPath("/apprenticeships")
                    .UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithRegistrationFirstSeenOn()
        {
            _server.Given(
                    Request.Create()
                        .WithPath("/registrations/*/firstseen")
                        .UsingPost()
                         )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Accepted));

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithUserAccount()
        {
            _server.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*"))
                    .AtPriority(3)
                .RespondWith(Response.Create()
                    .WithBodyAsJson(new Apprentice
                    {
                        FirstName = "Bob",
                        LastName = "Bobbertson",
                        DateOfBirth = new DateTime(2000, 01, 13),
                        ApprenticeId = Guid.NewGuid(),
                        Email = "bob@bobbertson.com",
                        TermsOfUseAccepted = true,
                    }));
            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithUsersApprenticeships()
        {
            _server.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/*/apprenticeships"))
                    .AtPriority(2)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Apprenticeships = new[]
                        {
                            new { Id = 1235, EmployerName = "My Mock company" },
                        }
                    }));

            _server.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/*/apprenticeships/1235"))
                    .AtPriority(1)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new Apprenticeship
                    {
                        Id = 1235,
                        EmployerName = "My Mock company",
                        TrainingProviderName = "My Mock trainer",
                        CourseName = "My mock apprenticeship course",
                        CourseOption = (string)null,
                        CourseLevel = 3,
                        PlannedStartDate = new DateTime(2021, 03, 12),
                        PlannedEndDate = new DateTime(2022, 09, 15),
                        DurationInMonths = 19,
                        ConfirmBefore = DateTime.UtcNow.AddDays(12).AddDays(1),
                        ConfirmedOn = (DateTime?)null,
                        EmployerCorrect = true,
                        TrainingProviderCorrect = false,
                        RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed
                                                                | RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed
                                                                | RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                        ApprenticeshipDetailsCorrect = (bool?)true,
                        HowApprenticeshipDeliveredCorrect = (bool?)true,
                        RecognisePriorLearning = false,
                        RevisionId = 9008
                    }));

            _server
                .Given(Request.Create()
                    .UsingPatch()
                    .WithPath("/apprentices/*/apprenticeships/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithMyApprenticeship()
        {
            _server.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/*/confirmed/latest")
                )
                .AtPriority(1)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new Apprenticeship
                    {
                        Id = 1235,
                        EmployerName = "My Mock company",
                        TrainingProviderName = "My Mock trainer",
                        CourseName = "My confirmed apprenticeship course",
                        CourseOption = (string)null,
                        CourseLevel = 3,
                        PlannedStartDate = new DateTime(2021, 03, 12),
                        PlannedEndDate = new DateTime(2022, 09, 15),
                        EmploymentEndDate = new DateTime(2022, 02, 15),
                        DeliveryModel = DeliveryModel.PortableFlexiJob,
                        DurationInMonths = 19,
                        EmployerCorrect = true,
                        TrainingProviderCorrect = true,
                        RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed
                                                                | RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed
                                                                | RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                        ApprenticeshipDetailsCorrect = true,
                        HowApprenticeshipDeliveredCorrect = true,
                        RevisionId = 9009
                    }));

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithMyApprenticeshipAndSpecificRevision()
        {
            _server.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/*/revisions/9009")
                )
                .AtPriority(1)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new Apprenticeship
                    {
                        Id = 1235,
                        EmployerName = "My Previous Mock company",
                        TrainingProviderName = "My Mock trainer",
                        CourseName = "My confirmed apprenticeship course",
                        CourseOption = (string)null,
                        CourseLevel = 3,
                        PlannedStartDate = new DateTime(2021, 03, 12),
                        PlannedEndDate = new DateTime(2022, 09, 15),
                        EmploymentEndDate = new DateTime(2022, 02, 15),
                        DeliveryModel = DeliveryModel.PortableFlexiJob,
                        DurationInMonths = 19,
                        EmployerCorrect = true,
                        TrainingProviderCorrect = true,
                        RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed 
                                                                | RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed 
                                                                | RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed,
                        ApprenticeshipDetailsCorrect = true,
                        HowApprenticeshipDeliveredCorrect = true,
                        ConfirmedOn = new DateTime(2022, 03, 01),
                        RevisionId = 9009
                    }));

            return this;
        }


        public ApprenticeCommitmentsApiBuilder WithEmployerConfirmation()
        {
            _server.Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath($"/apprentices/*/apprenticeships/*/employerconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                            );

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithTrainingProviderConfirmation()
        {
            _server.Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath($"/apprentices/*/apprenticeships/*/trainingproviderconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                            );

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithApprenticeshipDetailsConfirmation()
        {
            _server.Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath($"/apprentices/*/apprenticeships/*/apprenticeshipdetailsconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                            );

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithRolesAndResponsibilitiesConfirmation()
        {
            _server.Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath($"/apprentices/*/apprenticeships/*/rolesandresponsibilitiesconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                            );

            return this;
        }

        public ApprenticeCommitmentsApiBuilder WithHowApprenticeshipWillBeDeliveredConfirmation()
        {
            _server.Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath($"/apprentices/*/apprenticeships/*/howapprenticeshipwillbedeliveredconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                            );

            return this;
        }
    }
}