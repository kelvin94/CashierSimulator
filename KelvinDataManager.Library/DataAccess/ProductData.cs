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
    
    public class ProductData
    {
        private readonly IConfiguration config;

        public ProductData(IConfiguration config)
        {
            this.config = config;
        }

        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "KelvinData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "KelvinData").FirstOrDefault();

            return output;
        }
    }
}
