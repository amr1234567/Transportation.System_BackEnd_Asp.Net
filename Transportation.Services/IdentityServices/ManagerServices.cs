using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Transportation.Interfaces.IIdentityServices;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Identity;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Constants;
using Transportation.Core.Helpers.Functions;

namespace Transportation.Services.IdentityServices
{
    public class ManagerServices(
        UserManager<BusStopManger> userManager,
        ITokenService tokenService) : IManagerServices
    {

        public async Task<ResponseModel<TokenModel>> SignIn(LogInDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await userManager.FindByEmailAsync(model.Email) ??
                throw new NullReferenceException("Email or Password Wrong");
            var check = await userManager.CheckPasswordAsync(user, model.Password);
            if (!check)
                throw new NullReferenceException("Email or Password Wrong");

            var userRoles = await userManager.GetRolesAsync(user);
            var token = await tokenService.CreateToken(user, [.. userRoles]);
            return new ResponseModel<TokenModel>()
            {
                StatusCode = 200,
                Body = token,
                Message = "Logged In"
            };
        }

        public async Task<bool> SignUp(SignUpAsManagerDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var checkingUser = await userManager.FindByEmailAsync(model.Email);

            if (checkingUser != null)
                throw new Exception("Email Exist for another account");

            var newManager = new BusStopManger
            {
                Email = model.Email,
                Name = model.Name,
                UserName = new MailAddress(model.Email).User
            };

            var response = await userManager.CreateAsync(newManager, model.Password);
            if (!response.Succeeded)
                throw new Exception("Something went wrong");


            var user = await userManager.FindByEmailAsync(newManager.Email) ??
                throw new Exception("Something went wrong");

            var res2 = await userManager.AddToRoleAsync(user, Roles.BusStopManager);
            if (!res2.Succeeded)
                throw new Exception($"Can't add the user for role {Roles.BusStopManager}");

            return true;

        }

        public async Task enrollBusStop(string startBusStopId, string destinationBusStopId)
        {
            var startBusStop = await userManager.Users
                .Include(bs => bs.BusStops)
                .FirstOrDefaultAsync(bs => bs.Id.Equals(startBusStopId));

            if (startBusStop?.BusStops is null)
                throw new Exception($"Bus stop with Id {startBusStopId} Doesn't Exist");

            var destinationBusSto = await userManager.Users.Include(bs => bs.BusStops)
                .FirstOrDefaultAsync(bs => bs.Id.Equals(destinationBusStopId));

            if (destinationBusSto?.BusStops is null)
                throw new Exception($"Bus stop with Id {destinationBusStopId} Doesn't Exist");

            startBusStop.BusStops = [.. startBusStop.BusStops, destinationBusSto];
            destinationBusSto.BusStops = [.. destinationBusSto.BusStops, startBusStop];

            await userManager.UpdateAsync(startBusStop);
            await userManager.UpdateAsync(destinationBusSto);
        }

        public Task<IEnumerable<ReturnedBusStopDto>> GetAllStartBusStops()
        {
            var busStops = userManager.Users
                .Select(x => x.FromBusStopToBusStopDto()).AsNoTracking().AsEnumerable();
            return Task.FromResult(busStops);
        }

        public async Task<ReturnedBusStopDto> GetBusStop(string id)
        {
            var busStop = await userManager.Users.Include(bsm => bsm.BusStops)
                              .FirstOrDefaultAsync(bsm => bsm.Id == id) ??
                          throw new Exception($"Bus stop with Id {id} Doesn't Exist");

            return busStop.FromBusStopToBusStopDto();
        }

        public async Task<IEnumerable<ReturnedBusStopDto>> GetAllDestinationBusStops(string startBusStopId)
        {
            var startBusStop = await userManager.Users.Include(bs => bs.BusStops)
                .FirstOrDefaultAsync(bs => bs.Id.Equals(startBusStopId));
            if (startBusStop == null)
                throw new NullReferenceException($"Bus Stop with id '{startBusStop}' doesn't exist");

            if (startBusStop.BusStops == null || !startBusStop.BusStops.Any())
                return [];

            var busStops = startBusStop.BusStops
                .Select(x => x.FromBusStopToBusStopDto());
            return busStops;
        }
    }
}
