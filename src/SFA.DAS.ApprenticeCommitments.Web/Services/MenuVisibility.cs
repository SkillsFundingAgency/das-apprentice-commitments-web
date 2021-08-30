using SFA.DAS.ApprenticePortal.SharedUi.Services;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    internal class MenuVisibility : IMenuVisibility
    {
        public async Task<bool> ShowConfirmMyApprenticeship() => true;
    }
}