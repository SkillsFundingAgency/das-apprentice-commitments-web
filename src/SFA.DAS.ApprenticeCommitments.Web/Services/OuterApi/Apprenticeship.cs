namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public string EmployerName { get; set; }
        public string TrainingProviderName { get; set; }
        public bool? EmployerCorrect { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? RolesAndResponsibilitiesCorrect { get; set; }
    }
}