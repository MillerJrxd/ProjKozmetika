using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika.Classes
{
    public class DisplayService
    {
        public byte SzolgáltatásID { get; set; }
        public string SzolgáltatásKategória { get; set; }
        public TimeSpan SzolgáltatásIdeje { get; set; }
        public int SzolgáltatásÁra { get; set; }
    }
}
