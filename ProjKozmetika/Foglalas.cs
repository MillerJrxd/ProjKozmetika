using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika
{
    public class Foglalas
    {
        DateTime foglalasStart, foglalasEnd;

        public Foglalas()
        {

        }

        public DateTime FoglalasStart { get => foglalasStart; set => foglalasStart = value; }
        public DateTime FoglalasEnd { get => foglalasEnd; set => foglalasEnd = value; }
    }
}
