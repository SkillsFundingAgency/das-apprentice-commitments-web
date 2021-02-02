using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var apprenticeCommitmentsApi = ApprenticeCommitmentsApiBuilder.Create(8088)
                .WithUsersFirstLogin()
                .Build();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

        }
    }
}
