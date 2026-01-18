using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class MedicineModel
    {
        public string Id { get; set; }               // id NVARCHAR(10)
        public string Name { get; set; }             // name NVARCHAR(30)
        public int? Content { get; set; }            // content INT
        public string ActiveIngredient { get; set; } // activeIngredient NVARCHAR(30)
        public DateTime? InputTime { get; set; }     // inputTime DATE
        public DateTime? ProductionDate { get; set; }// productionDate DATE
        public DateTime? ExpirationDate { get; set; }// expirationDate DATE
        public string Unit { get; set; }             // unit NVARCHAR(10)
        public string Tid { get; set; }              // tid NVARCHAR(10) (TypeMedicine id)
        public string Mid { get; set; }              // mid NVARCHAR(10) (Manufacturer id)
        public int Quantity { get; set; }            // quantity INT
        public int ImportPrice { get; set; }         // importPrice INT
        public int SellingPrice { get; set; }        // sellingPrice INT
        public string ImagePath { get; set; }         // imagePath NVARCHAR(200)
    }
}
