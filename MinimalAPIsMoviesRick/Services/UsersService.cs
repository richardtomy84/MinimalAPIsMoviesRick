﻿using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMoviesRick.Services
{
    public class UsersService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager) : IUsersService
    {
        public async Task<IdentityUser?> GetUser()
        {
            var emailClaim = httpContextAccessor.HttpContext!.
                User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (emailClaim != null)
            {
                return null;
            }

            var email = emailClaim.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
    ''