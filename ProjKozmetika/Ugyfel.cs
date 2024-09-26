using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika
{
    public class Ugyfel
    {
        string ugyfelFirstName, ugyfelLastName, ugyfelTel, ugyfelEmail;
        int pontok;

        public Ugyfel()
        {

        }

        public string UgyfelFirstName { get => ugyfelFirstName; set => ugyfelFirstName = value; }
        public string UgyfelLastName { get => ugyfelLastName; set => ugyfelLastName = value; }
        public string UgyfelTel { get => ugyfelTel; set => ugyfelTel = value; }
        public string UgyfelEmail { get => ugyfelEmail; set => ugyfelEmail = value; }
        public int Pontok { get => pontok; set => pontok = value; }
    }
}
