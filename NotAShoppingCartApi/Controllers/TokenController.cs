using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotAShoppingCartApi.Data;

namespace NotAShoppingCartApi.Controllers
{
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)
        {
            if(await isValidUsernameAndPassword(username, password))
            {
                /****
                 * When talk about controllers for web MVC, we r returning a page.
                 * But here we are in the API, so we return an object, in this Create() method
                 * "ObjectResult" is an implementation of "IActionResult"
                 */
                return new ObjectResult(await GenerateToken(username));
            } else
            {
                return BadRequest();
            }
        }

        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await userManager.FindByEmailAsync(username);
            var roles = from ur in context.UserRoles
                        join r in context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };
            // Claims are pieces of information
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(14)).ToUnixTimeSeconds().ToString())

            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            }
            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mySecretKey!@#$GGininder")),
                        SecurityAlgorithms.HmacSha256)),
                    new JwtPayload(claims));
            var output = new
            {
                // create a string from our token
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = username
            };
            return output;

        }

        private async Task<bool> isValidUsernameAndPassword(string username, string password)
        {
            var user = await userManager.FindByEmailAsync(username);
            return await userManager.CheckPasswordAsync(user, password);
        }
    }
}