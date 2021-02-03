using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests
{
    public class ApprenticeCommitmentsWeb : IDisposable
    {
        public HttpClient Client { get; private set; }
        public HttpResponseMessage Response { get; set; }
        public Uri BaseAddress { get; private set; }
        public IHook<IActionResult> ActionResultHook { get; set; }

        private bool isDisposed;

        public ApprenticeCommitmentsWeb(HttpClient client, IHook<IActionResult> actionResultHook)
        {
            Client = client;
            BaseAddress = client.BaseAddress;
            ActionResultHook = actionResultHook;
        }

        public async Task Get(string url)
        {
            Response?.Dispose();
            Response = await Client.GetAsync(url);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                Response?.Dispose();
            }

            isDisposed = true;
        }
    }
}