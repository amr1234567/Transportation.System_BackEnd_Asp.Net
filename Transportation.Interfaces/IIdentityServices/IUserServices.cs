using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Interfaces.IIdentityServices
{
    public interface IUserServices
    {
        Task<ResponseModel<string>> SignUp(SignUpDto model);//, string Token);
        Task<ResponseModel<TokenModel>> SignIn(LogInDto model);
        Task<bool> ConfirmEmail(string Email, string Token);
        Task<bool> ConfirmPhoneNumber(string email, string PhoneNumber, string ConfirmToken);
        Task<ResponseModel<string>> ResetPasswordConfirmation(ResetPasswordDto model);
        Task<ResponseModel<string>> ResetPassword(string Email);
        Task<ResponseModel<bool>> VerifyChangePhoneNumber(string Email);
        Task<ResponseModel<bool>> ChangePhoneNumber(ChangePhoneNumberDto model);
        Task<ResponseModel<bool>> EditPersonalData(EditPersonalDataDto model, string userId);
    }
}
