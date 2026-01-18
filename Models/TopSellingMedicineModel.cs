using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class TopSellingMedicineModel
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int TotalSold { get; set; }
    }
}
