using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Helpers.Classes;
using Transportation.Core.Identity;
using Transportation.Interfaces.IIdentityServices;

namespace Transportation.Services.IdentityServices
{
    public class TokenService(IOptions<JwtHelper> config) : ITokenService
    {
        private SymmetricSecurityKey _key = new(new byte[10]);
        private readonly JwtHelper _config = config.Value;

        public Task<TokenModel> CreateToken(User user, List<string> roles, List<Claim>? internalClaims = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (roles is null || roles.Count == 0)
                throw new Exception("Roles Can't be null");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (internalClaims is not null)
                claims = (List<Claim>)claims.Union(internalClaims);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));

            var expiredOn = DateTime.UtcNow.AddMinutes(_config.expirePeriodInMinuts);

            var securityToken = new JwtSecurityToken(
                issuer: _config.issuer,
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature),
                claims: claims,
                expires: expiredOn
                );

            return Task.FromResult(new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                TokenExpiration = expiredOn
            });
        }

    }
}
