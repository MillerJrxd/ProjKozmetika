namespace ProjKozmetika.Classes
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
