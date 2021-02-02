using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public class ApprenticeCommitmentsApiBuilder 
    {
        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        private readonly WireMockServer _server;

        public ApprenticeCommitmentsApiBuilder(int port)
        {
            _server = WireMockServer.StartWithAdminInterface(port, true);
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
            var data = new TestData.NewAccountDetails();
            var response = JsonConvert.SerializeObject(data.Response, DefaultSerializerSettings);

            _server.Given(
                Request.Create()
                    .WithPath($"/registrations/{data.RegistrationId}")
                    .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(response)
                );

            return this;
        }


    }
}
