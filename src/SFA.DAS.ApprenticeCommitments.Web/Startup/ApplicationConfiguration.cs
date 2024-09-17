#nullable disable

using SFA.DAS.ApprenticePortal.SharedUi;
using SFA.DAS.ApprenticePortal.SharedUi.GoogleAnalytics;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticePortal.SharedUi.Zendesk;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class ApplicationConfiguration : ISharedUiConfiguration
    {
        public string ApprenticeCommitmentsBaseUrl { get; set; }
        public AuthenticationServiceConfiguration Authentication { get; set; }
        public DataProtectionConnectionStrings ConnectionStrings { get; set; }
        public OuterApiConfiguration ApprenticeCommitmentsApi { get; set; }
        public ZenDeskConfiguration Zendesk { get; set; }
        public GoogleAnalyticsConfiguration GoogleAnalytics { get; set; }
        public NavigationSectionUrls ApplicationUrls { get; set; }
        public bool UseGovSignIn { get; set; }
    }
}