using Transportation.Core.Dto.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Interfaces.IIdentityServices
{
    public interface IAdminServices
    {
        Task<bool> SignUp(SignUpAsAdminDto model);//, string Token);
        Task<ResponseModel<TokenModel>> SignIn(LogInDto model);
    }
}
