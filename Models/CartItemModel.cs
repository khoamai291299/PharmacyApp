using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class CartItemModel
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }

        public int Quantity { get; set; }
        public int UnitPrice { get; set; }

        public int TotalAmount => Quantity * UnitPrice;
    }
}

