﻿using KelvinDataManager.Library.DataAccess;
using KelvinDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TRMDataManager.Controllers
{
    [Authorize(Roles ="Cashier,Admin")]
    public class ProductController : ApiController
    {
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();

            return data.GetProducts();
        }
    }
}
