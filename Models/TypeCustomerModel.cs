using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class TypeCustomerModel
    {
        public string Id { get; set; }           // id NVARCHAR(10)
        public string Name { get; set; }         // name NVARCHAR(30)
        public int MinimumLevel { get; set; }   // minimumLevel INT
        public int MaximumLevel { get; set; }   // maximumLevel INT
    }
}
