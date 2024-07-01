using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using Transportation.Interfaces.IIdentityServices;
using Transportation.Core.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Constants;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Services.IdentityServices
{
    public class AdminServices(UserManager<ApplicationAdmin> userManager, ITokenService tokenService) : IAdminServices
    {
        public async Task<ResponseModel<TokenModel>> SignIn(LogInDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new NullReferenceException("Email or Password Wrong");

            var check = await userManager.CheckPasswordAsync(user, model.Password);
            if (!check)
                throw new NullReferenceException("Email or Password Wrong");

            var userRoles = await userManager.GetRolesAsync(user);
            var token = await tokenService.CreateToken(user, userRoles.ToList());
            return new ResponseModel<TokenModel>()
            {
                StatusCode = 200,
                Body = token,
                Message = "Logged In"
            };
        }

        public async Task<bool> SignUp(SignUpAsAdminDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var userChecking = await userManager.FindByEmailAsync(model.Email);

            if (userChecking != null)
                throw new Exception("Email Exist for another account");

            var appUser = new ApplicationAdmin
            {
                Email = model.Email,
                Name = model.Name,
                UserName = new MailAddress(model.Email).User
            };

            var response = await userManager.CreateAsync(appUser, model.Password);
            if (!response.Succeeded)
                throw new Exception("Something went wrong");


            var user = await userManager.FindByEmailAsync(appUser.Email);
            if (user == null)
                throw new Exception("Something went wrong");

            var res2 = await userManager.AddToRoleAsync(user, Roles.Admin);
            if (!res2.Succeeded)
                throw new Exception($"Can't add the user for role {Roles.Admin}");

            return true;
        }
    }
}
