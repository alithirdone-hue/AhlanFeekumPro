using AhlanFeekumPro.MobileResponses;
using System.Threading.Tasks;

namespace AhlanFeekumPro.UserProfiles
{
    public partial interface IUserProfilesAppService
    {
        //Write your custom code here...
        Task<MobileResponseDto> RegisterAsync(RegisterCreateDto input);
    }
}