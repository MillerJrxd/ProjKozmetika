using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika
{
    public class Dolgozo
    {
        string dolgFirstName, dolgLastName, dolgTel, dolgEmail;
        bool status;
        int szolgaltatas;

        public Dolgozo()
        {

        }

        public string DolgFirstName { get => dolgFirstName; set => dolgFirstName = value; }
        public string DolgLastName { get => dolgLastName; set => dolgLastName = value; }
        public string DolgTel { get => dolgTel; set => dolgTel = value; }
        public string DolgEmail { get => dolgEmail; set => dolgEmail = value; }
        public bool Status { get => status; set => status = value; }
        public int Szolgaltatas { get => szolgaltatas; set => szolgaltatas = value; }
    }
}
