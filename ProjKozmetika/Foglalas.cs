using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika
{
    public class Foglalas
    {
        int foglalasID;
        DateTime foglalasStart, foglalasEnd;

        public Foglalas()
        {

        }

        public int FoglalasID { get => FoglalasID; set => FoglalasID = value; }
        public DateTime FoglalasStart { get => foglalasStart; set => foglalasStart = value; }
        public DateTime FoglalasEnd { get => foglalasEnd; set => foglalasEnd = value; }
    }
}
