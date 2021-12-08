using SFA.DAS.ApprenticeCommitments.Web.UnitTests;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Bindings
{
    [Binding]
    [Scope(Tag = "outerApi")]
    public class OuterApi
    {
        public static MockApi Client { get; set; }

        private readonly TestContext _context;

        public OuterApi(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 1)]
        public void Initialise()
        {
            Client ??= new MockApi();
            _context.OuterApi = Client;
        }

        [AfterScenario()]
        public void CleanUp()
        {
            Client?.Reset();
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            Client?.Dispose();
            Client = null;
        }
    }
}