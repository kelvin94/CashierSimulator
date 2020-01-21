using KelvinDataManager.Library.Internal.DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelvinDataManager.Library.DataAccess
{
    public class InventoryData
    {
        private readonly IConfiguration config;

        public InventoryData(IConfiguration config)
        {
            this.config = config;
        }
        public List<InventoryModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "KelvinData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            sql.SaveData("dbo.spInventory_Insert", item, "KelvinData");


        }
    }
}
