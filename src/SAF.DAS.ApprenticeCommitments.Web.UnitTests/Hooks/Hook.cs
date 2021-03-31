using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks
{
    public class Hook<T> : IHook<T>
    {
        public Action<T> OnReceived { get; set; }
        public Action<T> OnProcessed { get; set; }
        public Action<Exception, T> OnErrored { get; set; }
    }
}
