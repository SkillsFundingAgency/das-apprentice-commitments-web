using System;
using System.Collections.Generic;
using System.Security.Claims;
using WireMock.Server;

namespace SFA.DAS.ApprenticeCommitments.Web.MockServer
{
    public class ApprenticeCommitmentsApi : IDisposable
    {
        private readonly WireMockServer _server;

        private bool _isDisposed;

        public ApprenticeCommitmentsApi(WireMockServer server)
        {
            _server = server;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                if (_server != null && _server.IsStarted)
                {
                    _server.Stop();
                }

                _server?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
