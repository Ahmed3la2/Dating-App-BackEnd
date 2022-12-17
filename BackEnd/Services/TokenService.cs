using BackEnd.Entities;
using BackEnd.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace BackEnd.Services
{
    public class TokenService : ItokenService
    {
        public readonly SymmetricSecurityKey _key;
        public readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }


        // Method To Create Token
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
           };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var cred =  new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var TokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(TokenDesc);

            return tokenHandler.WriteToken(token);
        }
    }
}
