using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
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

        public async Task<HttpResponseMessage> Get(string url)
        {
            Response?.Dispose();
            return Response = await Client.GetAsync(url);
        }

        internal async Task<HttpResponseMessage> Post(string url, HttpContent content)
        {
            Response?.Dispose();
            return Response = await Client.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage message)
        {
            Response?.Dispose();
            return Response = await Client.SendAsync(message);
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