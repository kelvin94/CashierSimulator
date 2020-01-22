using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KelvinDataManager.Library.DataAccess;
using KelvinDataManager.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace NotAShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Cashier,Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration config;

        public ProductController(IConfiguration config)
        {
            this.config = config;
        }
        [HttpGet]
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData(config);

            return data.GetProducts();
        }
    }
}