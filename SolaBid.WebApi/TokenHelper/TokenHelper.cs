using Microsoft.IdentityModel.Tokens;
using SolaBid.Business.Dtos.ApiDtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SolaBid.WebApi.TokenHelper
{
    public class TokenHelper
    {
        public const string Issuer = "SolaBid";
        public const string Audience = "SolaBidUser";
        public const string Secret = "OFRC1j9aaR2BvADxNWlG2pmuD392UfQBZZLM1fuzDEzDlEpSsn+btrpJKd3FfY855OMA9oK4Mc8y48eYUrVUSw==";
        //public static string GenerateToken(LoginDto login)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        //    var claimsIdentity = new ClaimsIdentity(new[] {
        //        new Claim(ClaimTypes.NameIdentifier, login.UserId),
        //        new Claim(ClaimTypes.Name, login.UserName),
        //        new Claim(ClaimTypes.System, login.SiteId.ToString())
        //    });
        //    var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        //    var claims = new Dictionary<string, object>();
        //    claims.Add(JwtRegisteredClaimNames.Sub, login.UserName);
        //    claims.Add(JwtRegisteredClaimNames.Email, login.UserName);
        //    claims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = claimsIdentity,
        //        Issuer = Issuer,
        //        Audience = Audience,
        //        Claims = claims,
        //        Expires = DateTime.UtcNow.AddYears(7),
        //        SigningCredentials = signingCredentials,
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}


        public static string GenerateToken(LoginDto login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, login.UserId),
                new Claim(ClaimTypes.Name, login.UserName),
                new Claim(ClaimTypes.System, login.SiteId.ToString())
            });

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, login.UserId),
            new Claim(ClaimTypes.Name, login.UserName),
            new Claim(ClaimTypes.System, login.SiteId.ToString())
            };

            var tokenDescriptor = new JwtSecurityToken(

                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signingCredentials
            );
            var token = tokenHandler.WriteToken(tokenDescriptor);
            return token;
        }
    }
}
