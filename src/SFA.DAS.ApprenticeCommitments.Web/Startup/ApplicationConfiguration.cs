#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class ApplicationConfiguration
    {
        public AuthenticationServiceConfiguration Authentication { get; set; }
        public DataProtectionConnectionStrings ConnectionStrings { get; set; }
        public OuterApiConfiguration ApprenticeCommitmentsApi { get; set; }
        public HashingConfiguration Hashing { get; set; }
    }
}