using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class DetailedAttendanceModel
    {
        public int Id { get; set; }
        public string Eid { get; set; }
        public DateTime Days { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
