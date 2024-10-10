using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika.Classes
{
    public class DisplayReservation
    {
        public int FoglalasID { get; set; }
        public string UgyfelNev { get; set; }
        public string SzolgaltatasNev { get; set; }
        public string DolgozoNev { get; set; }
        public TimeSpan FoglalasStart { get; set; }

    }
}
