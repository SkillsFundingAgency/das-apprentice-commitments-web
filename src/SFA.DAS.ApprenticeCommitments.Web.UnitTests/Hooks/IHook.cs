using System;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks
{
    public interface IHook
    {
    }

    public interface IHook<T> : IHook
    {
        Action<T> OnReceived { get; set; }
        Action<T> OnProcessed { get; set; }
        Action<Exception, T> OnErrored { get; set; }
    }
}