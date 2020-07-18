using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarbonKitchen.Api.Data.Auth
{
    public class AuthorizedUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Token { get; set; }

    }
}
