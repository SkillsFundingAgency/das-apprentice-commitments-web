using System;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer.TestData
{
    public class NewAccountDetails
    {
        public Guid RegistrationId => new Guid("78af2c2e-87f9-4a1b-9b70-da9e0c432fbf");
        public string Email => "test@test.com";

        public RegistrationResponse Response => new RegistrationResponse
            {Email = this.Email, RegistrationId = this.RegistrationId};
    }

    // TODO This class belongs to the web app and will be replaced...
    public class RegistrationResponse
    {
        public Guid RegistrationId { get; set; }
        public string Email { get; set; }
    }
}