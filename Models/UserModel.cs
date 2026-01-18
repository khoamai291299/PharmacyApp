using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Eid { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }


        public bool IsAdmin => Role == "Admin";
        public bool IsSeller => Role == "Seller";
        public bool IsWarehouse => Role == "Warehouse";
        public bool IsActive => Status == "Active";
    }

}
