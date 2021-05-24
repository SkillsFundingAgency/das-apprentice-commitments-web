using System;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ApprenticeCommitmentsApiBuilder.Create(5121)
                .WithRegistrationFirstSeenOn()
                .WithUsersFirstLogin()
                .WithUsersApprenticeships()
                .WithEmployerConfirmation()
                .WithTrainingProviderConfirmation()
                .WithApprenticeshipDetailsConfirmation()
                .WithHowApprenticeshipWillBeDeliveredConfirmation()
                .WithRolesAndResponsibilitiesConfirmation()
                .Build();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();
        }
    }
}