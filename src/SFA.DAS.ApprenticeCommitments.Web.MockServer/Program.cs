using System;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var apprenticeCommitmentsApi = ApprenticeCommitmentsApiBuilder.Create(5121)
                .WithUsersFirstLogin()
                .WithUsersApprenticeships()
                .WithEmployerConfirmation()
                .WithTrainingProviderConfirmation()
                .WithApprenticeshipDetailsConfirmation()
                .WithHowApprenticeshipWillBeDeliveredConfirmation()
                .Build();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

        }
    }
}
