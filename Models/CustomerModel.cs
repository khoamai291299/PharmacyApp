using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class CustomerModel
    {
        public string Id { get; set; }               // id NVARCHAR(10)
        public string Name { get; set; }             // name NVARCHAR(50)
        public string Phone { get; set; }            // phone NVARCHAR(15)
        public string Address { get; set; }          // address NVARCHAR(100)
        public string Tid { get; set; }              // tid NVARCHAR(10) (TypeCustomer id)
        public int TotalExpenditure { get; set; }   // totalExpenditure INT
        public int CumulativePoints { get; set; }
    }
}
