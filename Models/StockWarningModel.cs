using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class StockWarningModel
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string WarningType { get; set; }
    }

}
