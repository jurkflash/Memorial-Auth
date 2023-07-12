using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sso.Core;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sso
{
    class ClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsProfileService(
            UserManager<IdentityUser> userManager, 
            IUserClaimsPrincipalFactory<IdentityUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            foreach (var userClaim in userClaims)
            {
                claims.Add(new Claim(userClaim.Type, userClaim.Value));
            }

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            //var userClaims = await _userManager.GetClaimsAsync(user);

            ////for visitors, they need to complete their profile first
            //if (context.Subject.FindFirst(ClaimTypes.Role)?.Value == Role.Visitor && userClaims.FirstOrDefault(a => a.Type == "fullname") == null)
            //{
            //    context.IsActive = false;
            //}
            //else
            //{
                context.IsActive = user != null;
            //}
        }
    }
}
