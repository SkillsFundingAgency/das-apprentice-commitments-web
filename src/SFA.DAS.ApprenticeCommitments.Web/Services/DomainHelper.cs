namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class DomainHelper
    {
        public string ParentDomain { get; }
        public bool Secure { get; }

        public DomainHelper(string parentDomain)
        {
            ParentDomain = parentDomain;
            Secure = parentDomain != ".localhost";
        }
    }
}
