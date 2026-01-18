using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class BillModel
    {
        public string Id { get; set; }               // id NVARCHAR(10)
        public DateTime? DateOfcreate { get; set; } // dateOfcreate DATETIME
        public string Cid { get; set; }              // cId NVARCHAR(10)
        public string Eid { get; set; }              // eId NVARCHAR(10)
        public string PromotionId { get; set; }
        public int TotalAmount { get; set; }        // totalAmount INT
    }
}
