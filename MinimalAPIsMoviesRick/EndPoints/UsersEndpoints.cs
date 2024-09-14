﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Filters;
using MinimalAPIsMoviesRick.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MinimalAPIsMoviesRick.EndPoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
        {
            group.MapPost("/", Register)
                .AddEndpointFilter<ValidationFilter<UserCredentialsDTO>>();
            return group; 
        
        }

        static async Task<Results<Ok<AuthenticationResponseDTO> , 
            BadRequest<IEnumerable<IdentityError>>>> Register(UserCredentialsDTO userCredentialsDTO,
                         [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = new IdentityUser

            {
                UserName = userCredentialsDTO.Email,
                Email = userCredentialsDTO.Email,   
            };
            var result = await userManager.CreateAsync(user,userCredentialsDTO.Password);

            if (result.Succeeded)
            {
                var authentionResponse = 
                    await BuildToken(userCredentialsDTO, configuration,
                    userManager);
                return TypedResults.Ok(authentionResponse);
            }
            else
            { 
                return TypedResults.BadRequest(result.Errors);
            
            }
             
        }

        private async static Task<AuthenticationResponseDTO> BuildToken(UserCredentialsDTO userCredentialsDTO,
            IConfiguration configuration,UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email",userCredentialsDTO.Email),
                new Claim("Whatever I want","this is a value")
            };

            var user = await userManager.FindByNameAsync(userCredentialsDTO.Email);
            var claimsFromDB = await userManager.GetClaimsAsync(user!);
                
            claims.AddRange(claimsFromDB); 
            var key = KeysHandler.GetKey(configuration).First();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer:null,audience:null, 
                claims:claims, expires: expiration, signingCredentials:credentials );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO
            {
                Token = token,
                Expiration = expiration,
            };
        }
    }
}
