using KelvinDataManager.Library.Internal.DataAccess;
using KelvinDataManager.Library.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelvinDataManager.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration config;

        public SaleData(IConfiguration config)
        {
            this.config = config;
        }
        public void SaveSale(SaleModel saleAPIInput, string cashierId)
        {
            System.Diagnostics.Debug.WriteLine("Start SaveSale");
            // Filling in the sale deal models we'll save to the db
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData productDataAccess = new ProductData(config);
            foreach(var item in saleAPIInput.SaleDetails)
            {
                // Fill in the available info 
                System.Diagnostics.Debug.WriteLine("item.ProductId "+ item.ProductId);
                System.Diagnostics.Debug.WriteLine(" item.Quantity " + item.Quantity);
                System.Diagnostics.Debug.WriteLine(" cashierId " + cashierId);

                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                var productInfo = productDataAccess.GetProductById(item.ProductId);
                System.Diagnostics.Debug.WriteLine("productInfo " + productInfo);
                System.Diagnostics.Debug.WriteLine("productInfo RetailPrice " + productInfo.RetailPrice);

                if (productInfo == null)
                {
                    throw new Exception($"The product id of {detail.ProductId} is not found in db");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);
                details.Add(detail);
            }


            // create the sale model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = 0,
                CashierId = cashierId
                
            };
            sale.Total = sale.SubTotal;
            // save the model
            using (SqlDataAccess sql = new SqlDataAccess(config))
            {
                try
                {
                    sql.StartTransaction("KelvinData");
                    sql.SaveDataInTransaction<SaleDBModel>("dbo.spSale_Insert", sale);

                    // Get the id from sale model

                    int saleId = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();

                    // Finish filling in sale detail models.
                    foreach (var item in details)
                    {
                        item.SaleId = saleId;
                        // save the sale detail models
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                } catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }



           
            

        }

        public List<SaleReportModel> GetSalesReport()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "KelvinData");

            return output;
        }
        
    }
}
