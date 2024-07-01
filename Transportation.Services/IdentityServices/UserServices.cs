
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Constants;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Identity;
using Transportation.Interfaces.IHelpersServices;
using Transportation.Interfaces.IIdentityServices;
using Twilio.Rest.Api.V2010.Account;

namespace Transportation.Services.IdentityServices
{
    public class UserServices(
        UserManager<ApplicationUser> userManager,
        IMailServices mailServices,
        ISmsSevices smsServices,
        ITokenService tokenService)
        : IUserServices
    {
        private readonly IMailServices _mailServices = mailServices;

        public async Task<ResponseModel<TokenModel>> SignIn(LogInDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await userManager.FindByEmailAsync(model.Email) ??
                       throw new Exception("Email or Password Wrong");
            var check = await userManager.CheckPasswordAsync(user, model.Password);
            if (!check)
                throw new Exception("Email or Password Wrong");

            var userRoles = await userManager.GetRolesAsync(user);
            var token = await tokenService.CreateToken(user, [.. userRoles]);
            return new ResponseModel<TokenModel>()
            {
                StatusCode = 200,
                Body = token,
                Message = "Logged In"
            };
        }



        public async Task<ResponseModel<string>> SignUp(SignUpDto model)//, string UrlToAction)
        {
            ArgumentNullException.ThrowIfNull(model);

            var checkingUser = await userManager.FindByEmailAsync(model.Email);

            if (checkingUser != null)
                throw new Exception("Email Exist for another account");

            var newUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
            };

            var response = await userManager.CreateAsync(newUser, model.Password);
            if (!response.Succeeded)
                throw new Exception("Something went wrong");

            var user = await userManager.FindByEmailAsync(newUser.Email) ?? throw new Exception("Something went wrong");
            var res = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

            if (!res.Succeeded)
                throw new Exception("Can't Save Phone Number");

            #region Add Role

            var res2 = await userManager.AddToRoleAsync(user, Roles.User);
            if (!res2.Succeeded)
                throw new Exception($"Can't add the user for role {Roles.User}");

            #endregion

            #region Phone Verification

            var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            var result = smsServices.Send($"Verification Code : {code}", model.PhoneNumber);

            if (result.Status == MessageResource.StatusEnum.Sending || result.Status == MessageResource.StatusEnum.Queued)
                return new ResponseModel<string>
                {
                    StatusCode = 200,
                    Message = "Registering operation succeeded, please confirm your phone number",
                    Body = code
                };
            else
                return new ResponseModel<string>
                {
                    StatusCode = 500,
                    Message = "SomeThing Went Wrong, Please Try Again"
                };
            #endregion

            #region Email Verification
            //---------------------------------------------------------------------
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(User);
            //var url = UrlToAction + $"?token={token}&email={User.Email}";
            //var message = new Message(
            //    new List<string> { User.Email },
            //    "Confirm Your Email",
            //    $"Click on the url : {url}"
            //    );
            //_mailServices.SendEmail(message); 
            #endregion
        }

        public async Task<bool> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                throw new Exception("Input Can't be null");

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception($"Account with Email {email} Not Found");

            var response = await userManager.ConfirmEmailAsync(user, token);
            if (!response.Succeeded)
                throw new Exception($"Something went wrong");

            return true;
        }

        public async Task<bool> ConfirmPhoneNumber(string email, string phoneNumber, string confirmToken)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(confirmToken))
                throw new Exception("Input Can't be null");

            #region  With VerificationCheckResource
            //var verification = await VerificationCheckResource.CreateAsync(
            //      to: PhoneNumber,
            //      code: Code,
            //      pathServiceSid: _TwilioSettings.Value.VerificationServiceSID
            //  );

            //if (verification.Status == "approved")
            //{
            //    var identityUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(PhoneNumber));
            //    identityUser.PhoneNumberConfirmed = true;
            //    var updateResult = await _userManager.UpdateAsync(identityUser);

            //    if (updateResult.Succeeded)
            //    {
            //        return true;
            //    }
            //}
            //return false; 
            #endregion

            //var user = await FindUserByPhoneNumberAsync(PhoneNumber);
            var user = await userManager.FindByEmailAsync(email) ??
                       throw new Exception($"Can't find email with this input");

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                throw new Exception("this account didn't register any phone number");
            }

            var result = await userManager.VerifyChangePhoneNumberTokenAsync(user, confirmToken, user.PhoneNumber);
            if (result)
            {
                user.PhoneNumberConfirmed = true;
                var response = await userManager.UpdateAsync(user);

                if (!response.Succeeded)
                    throw new Exception($"Something went wrong");
            }
            else
                throw new Exception($"Verification Code is wrong");
            return result;
        }



        public async Task<ResponseModel<string>> ResetPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email) ??
                       throw new NullReferenceException("Can't Find Account with this email");

            //var code = _smsSevices.GenerateCode();

            //if (string.IsNullOrEmpty(code))
            //    throw new Exception("Something went wrong in GenerateCode Function");

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                throw new Exception("this account didn't register any phone number");
            }

            var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            var result = smsServices.Send($"Verification Code : {code}", user.PhoneNumber);

            if (result.Status == MessageResource.StatusEnum.Sending || result.Status == MessageResource.StatusEnum.Queued)
                return new ResponseModel<string>
                {
                    StatusCode = 200,
                    Body = code,
                    Message = "Verify Message Sent"
                };
            return new ResponseModel<string>
            {
                StatusCode = 400,
                Message = "SomeThing Went Wrong, Please Try Again"
            };

        }

        public async Task<ResponseModel<string>> ResetPasswordConfirmation(ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email) ??
                       throw new NullReferenceException("Can't Find Account with this email");

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                throw new Exception("this account didn't register any phone number");
            }

            var response = await userManager.VerifyChangePhoneNumberTokenAsync(user, model.code, user.PhoneNumber);
            if (!response)
                throw new Exception("Verification Code is Wrong");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            if (string.IsNullOrEmpty(token))
                throw new Exception("Something went wrong in Generate Code Service");

            var result = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (!result.Succeeded)
                throw new Exception("Can't reset Password");

            return new ResponseModel<string>
            {
                Message = "Password Reseted Successfully",
                StatusCode = 200
            };

        }



        public async Task<ResponseModel<bool>> EditPersonalData(EditPersonalDataDto model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId) ??
                       throw new NullReferenceException($"user with id '{userId}' can't be found");

            user.Email = string.IsNullOrEmpty(model.Email) ? user.Email : model.Email;
            user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded ?
                new ResponseModel<bool>
                {
                    StatusCode = 200,
                    Message = "Data Edited Successfully"
                } : new ResponseModel<bool>
                {
                    StatusCode = 500,
                    Message = "Something Went Wrong"
                };
        }



        public async Task<ResponseModel<bool>> VerifyChangePhoneNumber(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
                throw new NullReferenceException("Account with this Email Can't be found");
            var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            var result = smsServices.Send($"Verification Code : {code}", user.PhoneNumber);

            if (result.Status == MessageResource.StatusEnum.Accepted)
                return new ResponseModel<bool>
                {
                    StatusCode = 200,
                    Message = "Registering operation succeeded, please confirm your email"
                };
            else
                return new ResponseModel<bool>
                {
                    StatusCode = 500,
                    Message = "SomeThing Went Wrong, Please Try Again"
                };
        }

        public async Task<ResponseModel<bool>> ChangePhoneNumber(ChangePhoneNumberDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new NullReferenceException("Account with this email can't be found");
            var result = await userManager.VerifyChangePhoneNumberTokenAsync(user, model.Verifytoken, model.PhoneNumber);
            if (!result)
                throw new Exception("Verification Code is Wrong");
            user.PhoneNumber = model.PhoneNumber;
            var response = await userManager.UpdateAsync(user);
            return response.Succeeded ? new ResponseModel<bool>
            {
                StatusCode = 200,
                Body = true,
                Message = "Change your phone number has been done successfully"
            } : new ResponseModel<bool>
            {
                StatusCode = 500,
                Message = "Something went wrong while change your password, please try again"
            };
        }



        private async Task<ApplicationUser> FindUserByPhoneNumberAsync(string PhoneNumber)
        {
            if (string.IsNullOrEmpty(PhoneNumber))
                throw new ArgumentNullException("Input Can't be null");
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
            if (user == null)
                throw new NullReferenceException($"user with phone number '{PhoneNumber}' doesn't exist");
            return user;
        }


    }
}
