using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KelvinDataManager.Library.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace NotAShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration config;

        public InventoryController(IConfiguration config)
        {
            this.config = config;
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData(config);
            return data.GetInventory();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(config);
            data.SaveInventoryRecord(item);
        }
    }
}