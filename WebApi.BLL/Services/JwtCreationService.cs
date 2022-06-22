using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.BLL.Interfaces
{
    public class JwtCreationService : IJwtCreationService
    {
        private readonly IConfiguration configuration;

        public JwtCreationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// We are creating a new JWT token with the claims that we passed in
        /// </summary>
        /// <param name="claims">The claims that we want to pass in the token.</param>
        /// <returns>
        /// A JWT token.
        /// </returns>
        public JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            /* Creating a new SymmetricSecurityKey object with the secret key that we passed in. */
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));

            /* Creating a new JWT token with the claims that we passed in. */
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}