namespace ProjKozmetika.Classes
{
    public class Ugyfel
    {
        int id;
        string ugyfelFirstName, ugyfelLastName, ugyfelTel, ugyfelEmail;
        int pontok;

        public Ugyfel()
        {

        }

        public int ID { get => ID; set => ID = value; }
        public string UgyfelFirstName { get => ugyfelFirstName; set => ugyfelFirstName = value; }
        public string UgyfelLastName { get => ugyfelLastName; set => ugyfelLastName = value; }
        public string UgyfelTel { get => ugyfelTel; set => ugyfelTel = value; }
        public string UgyfelEmail { get => ugyfelEmail; set => ugyfelEmail = value; }
        public int Pontok { get => pontok; set => pontok = value; }

    }
}
