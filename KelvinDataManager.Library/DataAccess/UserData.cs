using KelvinDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using KelvinDataManager.Library.Internal.DataAccess;
using Microsoft.Extensions.Configuration;

namespace KelvinDataManager.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration config;

        public UserData(IConfiguration config)
        {
            this.config = config;
        }
        public List<UserModel> GetUserById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            //anonymous object
            var p = new { Id = Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "KelvinData");

            return output;
        }
    }
}
