using KelvinDataManager.Library.DataAccess;
using KelvinDataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            //string userId = RequestContext.Principal.Identity.GetUserId();
            string userId = "7962aed8-bfaf-41d6-bf73-bfbb4f81647c"; // TODO: 这里出问题了：register 用户是把info存入了"EFData" 这个db, 但是我们在这里query是query "KelvinData" DB.
            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSaleReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            SaleData data = new SaleData();
            return data.GetSalesReport();
        }
    }
}
