﻿namespace CarbonKitchen.Api.Services.Auth
{
    using CarbonKitchen.Api.Data.Auth;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AuthService : IAuthService
    {
        public Data.Auth.SecurityToken Authenticate(string keyAuth)
        {
            if (string.IsNullOrEmpty(keyAuth))
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("E546C8DF278CD5931069B522E695D4F2");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtSecurityToken = tokenHandler.WriteToken(token);

            return new Data.Auth.SecurityToken() { auth_token = jwtSecurityToken };
        }
    }
}
