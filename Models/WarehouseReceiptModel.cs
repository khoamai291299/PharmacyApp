using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class WarehouseReceiptModel
    {
        public string Id { get; set; }               // id NVARCHAR(10)
        public DateTime ? InputDay { get; set; }     // inputday DATE time
        public string Eid { get; set; }              // eid NVARCHAR(10) (Employee id)
        public int TotalImport { get; set; }        // totalImport INT
    }
}
