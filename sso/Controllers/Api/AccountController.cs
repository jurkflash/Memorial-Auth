using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Sso.Controllers.Api;
using Sso.Core;
using System.Linq;
using Sso.Services.Utils;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace Sso.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : BaseController
    {
        private readonly UserManager<CCAIdentity> _userManager;
        public AccountController(
            UserManager<CCAIdentity> userManager
            )
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount(string username, string email, string password, string name, string userId, string role)
        {
            var userInDb = await _userManager.FindByNameAsync(username);

            if (userInDb == null)
            {
                var user = new CCAIdentity
                {
                    EmailConfirmed = true,
                    UserName = username,
                    Email = email,
                    Name = name,
                    CreatedUtcDatetime = DateTime.UtcNow,
                    LastModifiedUtcDateTime = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, UtilService.Decrypt(password));

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(a => a.Description));
                }

                result = await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim("UserId", userId),
                });

                return Ok(user.Id);
            }

            return BadRequest("username exists");
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var userInDb = await _userManager.FindByNameAsync(username);

            if (userInDb != null)
            {
                var claims = await _userManager.GetClaimsAsync(userInDb);
                await _userManager.RemoveClaimsAsync(userInDb, claims);

                await _userManager.DeleteAsync(userInDb);

                return Ok();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPut("Password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(string username, string oldPassword, string newPassword)
        {
            var userInDb = await _userManager.FindByNameAsync(username);

            if (userInDb != null)
            {
                //get userid , if is admin proceed. else check if the userid belong to the username
                var claims = await _userManager.GetClaimsAsync(userInDb);
                var role = claims.Where(c => c.Type == ClaimTypes.Role).SingleOrDefault();
                if(role.Value != "Admin")
                {
                    var userId = claims.Where(c => c.Type == "UserId").SingleOrDefault();
                    if(userId.Value != GetCurrentUserId())
                    {
                        return BadRequest();
                    }
                }
                
                var result = await _userManager.ChangePasswordAsync(userInDb, UtilService.Decrypt(oldPassword), UtilService.Decrypt(newPassword));
                
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(a => a.Description));
                }

                return Ok();
            }

            return BadRequest("username not found");
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("Role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeRole(string username, string role)
        {
            var userInDb = await _userManager.FindByNameAsync(username);

            if (userInDb != null)
            {
                var claims = await _userManager.GetClaimsAsync(userInDb);
                var claimInDb = claims.Where(c => c.Type == ClaimTypes.Role).SingleOrDefault();

                if (claimInDb != null)
                {
                    await _userManager.ReplaceClaimAsync(userInDb, claimInDb, new Claim(ClaimTypes.Role, role));

                    return Ok();
                }
            }

            return BadRequest("username not found");
        }
    }
}
