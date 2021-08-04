namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsCompleted(this Apprenticeship apprenticeship) =>
            apprenticeship.ConfirmedOn != null;
    }
}