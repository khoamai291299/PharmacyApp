using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class SummaryAttendanceModel
    {
        public int Rid { get; set; }
        public string Eid { get; set; }
        public int NumOfworkDay { get; set; }
        public int NumOfdayOff { get; set; }
        public int NetSalary { get; set; }
    }
}
