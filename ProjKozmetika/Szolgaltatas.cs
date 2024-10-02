using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika
{
    public class Szolgaltatas
    {
        string szolgKategoria;
        TimeSpan szolgaltatasIdeje;
        int szolgAr;
        byte szolgID;

        public Szolgaltatas(byte szolgID, string szolgKategoria, TimeSpan szolgaltatasIdeje, int szolgAr)
        {
            SzolgKategoria = szolgKategoria;
            SzolgaltatasIdeje = szolgaltatasIdeje;
            SzolgAr = szolgAr;
            SzolgID = szolgID;
        }

        public string SzolgKategoria { get => szolgKategoria; set => szolgKategoria = value; }
        public TimeSpan SzolgaltatasIdeje { get => szolgaltatasIdeje; set => szolgaltatasIdeje = value; }
        public int SzolgAr { get => szolgAr; set => szolgAr = value; }
        public byte SzolgID { get => szolgID; set => szolgID = value; }
    }
}
