using KelvinDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using KelvinDataManager.Library.Internal.DataAccess;

namespace KelvinDataManager.Library.DataAccess
{
    public class UserData
    {
        public List<UserModel> GetUserById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess();

            //anonymous object
            var p = new { Id = Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "KelvinData");

            return output;
        }
    }
}
