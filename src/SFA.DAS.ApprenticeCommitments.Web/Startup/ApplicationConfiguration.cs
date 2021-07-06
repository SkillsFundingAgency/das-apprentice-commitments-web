#nullable disable

using SFA.DAS.Apprentice.SharedUi;
using SFA.DAS.Apprentice.SharedUi.GoogleAnalytics;
using SFA.DAS.Apprentice.SharedUi.Menu;
using SFA.DAS.Apprentice.SharedUi.Zendesk;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class ApplicationConfiguration : ISharedUiConfiguration
    {
        public string ApprenticeCommitmentsBaseUrl { get; set; }
        public AuthenticationServiceConfiguration Authentication { get; set; }
        public DataProtectionConnectionStrings ConnectionStrings { get; set; }
        public OuterApiConfiguration ApprenticeCommitmentsApi { get; set; }
        public HashingConfiguration Hashing { get; set; }
        public ZenDeskConfiguration Zendesk { get; set; }
        public GoogleAnalyticsConfiguration GoogleAnalytics { get; set; }
        public NavigationSectionUrls ApplicationUrls { get; set; }
    }
}