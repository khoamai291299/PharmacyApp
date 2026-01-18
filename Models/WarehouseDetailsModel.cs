using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class WarehouseDetailsModel
    {
        public string Wid { get; set; }           // wid NVARCHAR(10)
        public string Mid { get; set; }           // mid NVARCHAR(10)
        public int Quantity { get; set; }         // quantity INT
        public int UnitPrice { get; set; }      // unitPrice INT
        public int TotalAmount { get; set; }     // totalAmount INT
    }
}
