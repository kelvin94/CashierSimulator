using KelvinDataManager.Library.DataAccess;
using KelvinDataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        // GET: User
        

        // GET: User/Details/5
        public List<UserModel> GetById()
        {
            // get the id from the currently who's logged in
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();
            return data.GetUserById(userId);            
        }

        
    }
}
