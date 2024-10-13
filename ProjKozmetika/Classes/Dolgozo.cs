namespace ProjKozmetika.Classes
{
    public class Dolgozo
    {
        byte dolgozoId;
        string dolgFirstName, dolgLastName, dolgTel, dolgEmail;
        bool status;
        byte szolgaltatas;

        public Dolgozo(byte dolgozoId, string dolgFirstName, string dolgLastName, string dolgTel, string dolgEmail, bool status, byte szolgaltatas)
        {
            this.DolgozoId = dolgozoId;
            this.DolgFirstName = dolgFirstName;
            this.DolgLastName = dolgLastName;
            this.DolgTel = dolgTel;
            this.DolgEmail = dolgEmail;
            this.Status = status;
            this.Szolgaltatas = szolgaltatas;
        }

        public Dolgozo() { }

        public byte DolgozoId { get => dolgozoId; set => dolgozoId = value; }
        public string DolgFirstName { get => dolgFirstName; set => dolgFirstName = value; }
        public string DolgLastName { get => dolgLastName; set => dolgLastName = value; }
        public string DolgTel { get => dolgTel; set => dolgTel = value; }
        public string DolgEmail { get => dolgEmail; set => dolgEmail = value; }
        public bool Status { get => status; set => status = value; }
        public byte Szolgaltatas { get => szolgaltatas; set => szolgaltatas = value; }
        public string DolgozoFullName => $"{DolgLastName} {DolgFirstName}";
    }
}
