using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Identity;

namespace Transportation.Interfaces.IIdentityServices
{
    public interface ITokenService
    {
        Task<TokenModel> CreateToken(User user, List<string> roles, List<Claim>? Internalclaims = null);
    }
}
