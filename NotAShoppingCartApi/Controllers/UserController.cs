using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KelvinDataManager.Library.DataAccess;
using KelvinDataManager.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotAShoppingCartApi.Data;
using NotAShoppingCartApi.Models;

namespace NotAShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private readonly IConfiguration config;

        // Use Dependency Injection
        public UserController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            this.config = config;
        }


        [HttpGet]
        // GET: User/Details/5
        public List<UserModel> GetById()
        {
            // get the id from the currently who's logged in
            //string userId = RequestContext.Principal.Identity.GetUserId();
            // 上面找userId的方法是 .NET Framework的方法，升级到asp.net core后要用下面这个方法
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData(config);
            return data.GetUserById(userId);
        }

        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            
            var users = _context.Users.ToList();

            //var roles = _context.Roles.ToList();
            // Linq query that gives us user roles
            // _context.UserROles gives us "UserId | RoleId" table
            var userRoles = from userrole in _context.UserRoles
                            join r in _context.Roles on userrole.RoleId equals r.Id
                            select new { userrole.UserId, userrole.RoleId, r.Name }; // select new anonoymous object

            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };


                //foreach (var role in user.Roles)
                //{
                //    u.Roles.Add(role.RoleId, roles.Where(x => x.Id == role.RoleId).First().Name);
                //} // this forloop is upgraded to the next line
                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(x => x.RoleId, x => x.Name);


                output.Add(u);
            }
            return output;
            

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<String, String> GetAllRoles()
        {
            
                // Convert IdentityRole to Dictonary
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;
            
        }

        /***
         *Add a role to a user
         */
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pairing)
        {

            var user = await _userManager.FindByIdAsync(pairing.userId);

            await _userManager.AddToRoleAsync(user, pairing.roleName);
            
        }

        /***
         * Remove a role from a user
         */
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pairing)
        {

            var user = await _userManager.FindByIdAsync(pairing.userId);
            await _userManager.RemoveFromRoleAsync(user, pairing.roleName);
            
        }
    }
}