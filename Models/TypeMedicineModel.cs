using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class TypeMedicineModel
    {
        public string Id { get; set; }           // id NVARCHAR(10)
        public string Name { get; set; }         // name NVARCHAR(30)
        public string Description { get; set; } // description NVARCHAR(50)
    }
}
