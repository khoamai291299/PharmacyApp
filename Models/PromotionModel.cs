using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class PromotionModel
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RequiredPoints { get; set; }
        public int Quantity { get; set; }
        public int DiscountPercent { get; set; }

        public string Display => $"{Id} - Giảm {DiscountPercent}%";
    }
}
