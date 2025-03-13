namespace GridPath.Models.Parcels
{
    public class DruhPozemku
    {
        public DruhPozemku(string kod, string nazev)
        {
            Kod = kod;
            Nazev = nazev;
        }

        public string Kod { get; set; }
        public string Nazev { get; set; }
    }
}
