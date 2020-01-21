using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotAShoppingCartApi.Models;

namespace NotAShoppingCartApi.Controllers
{
    /*****
     * This is MVC controller, Inventory, Product, Sale, User Controller are API controllers.
     */
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(ILogger<HomeController> logger,
            RoleManager<IdentityRole> roleManager,
             UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            string[] roles = { "Manager", "Cashier", "Admin" };
            foreach(var role in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if(!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var user = await userManager.FindByEmailAsync("kelvin@lin.com");
            if(user != null)
            {
                await userManager.AddToRoleAsync(user, "Cashier");
                await userManager.AddToRoleAsync(user, "Admin");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
