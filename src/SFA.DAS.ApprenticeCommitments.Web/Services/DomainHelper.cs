namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class DomainHelper
    {
        public string ParentDomain { get; }
        public DomainHelper(string parentDomain)
        {
            ParentDomain = parentDomain;
        }
    }
}
