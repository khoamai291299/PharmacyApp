using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class ManufacturerModel
    {
        public string Id { get; set; }           // id NVARCHAR(10)
        public string Name { get; set; }         // name NVARCHAR(30)
        public string Country { get; set; }      // country NVARCHAR(20)
    }
}
