using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika.Classes
{
    public class DisplayWorker
    {
        public byte DolgozoID { get; set; }
        public string DolgozoNev { get; set; }
        public string DolgozoEmail { get; set; }
        public string DolgozoTel { get; set; }
        public bool DolgozoStatusz { get; set; }
        public string DolgozoSzolgaltatasa { get; set; }
        public byte DolgozoSzolgaltatasID { get; set; }
    }
}
