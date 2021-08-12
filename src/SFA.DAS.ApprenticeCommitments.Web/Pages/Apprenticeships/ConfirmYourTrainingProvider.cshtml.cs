using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    [HideNavigationBar]
    public class ConfirmYourTrainingModel : ApprenticeshipConfirmationPageModel
    {
        [BindProperty]
        public string TrainingProviderName { get; set; } = null!;

        public ConfirmYourTrainingModel(AuthenticatedUserClient client) : base("Provider", client)
        {
        }

        public override void LoadApprenticeship(Apprenticeship apprenticeship)
        {
            TrainingProviderName = apprenticeship.TrainingProviderName;
            Confirmed = apprenticeship.TrainingProviderCorrect;
        }

        public override ApprenticeshipConfirmationRequest CreateUpdate(bool confirmed)
            => ApprenticeshipConfirmationRequest.ConfirmTrainingProvider(confirmed);
    }
}