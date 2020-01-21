using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotAShoppingCartApi.Models
{
    public class ApplicationUserModel
    {
        // This is the model we will return to the FrontEnd or the response of /api/User/GetAllUsers
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
    }
}
