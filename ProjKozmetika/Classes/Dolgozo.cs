using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjKozmetika.Classes
{
    public class Dolgozo
    {
        byte id;
        string dolgFirstName, dolgLastName, dolgTel, dolgEmail;
        bool status;
        byte szolgaltatas;

        public Dolgozo(byte id, string dolgFirstName, string dolgLastName, string dolgTel, string dolgEmail, bool status, byte szolgaltatas)
        {

            Id = id;
            DolgFirstName = dolgFirstName;
            DolgLastName = dolgLastName;
            DolgTel = dolgTel;
            DolgEmail = dolgEmail;
            Status = status;
            Szolgaltatas = szolgaltatas;
        }

        public byte Id { get => id; set => id = value; }
        public string DolgFirstName { get => dolgFirstName; set => dolgFirstName = value; }
        public string DolgLastName { get => dolgLastName; set => dolgLastName = value; }
        public string DolgTel { get => dolgTel; set => dolgTel = value; }
        public string DolgEmail { get => dolgEmail; set => dolgEmail = value; }
        public bool Status { get => status; set => status = value; }
        public byte Szolgaltatas { get => szolgaltatas; set => szolgaltatas = value; }

        public string DolgozoFullName => $"{DolgLastName} {DolgFirstName}";
    }
}
