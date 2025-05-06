namespace GridPath.Models.Parcels
{
    public class ZpusobyOchrany
    {
        public ZpusobyOchrany(string kod, string nazev)
        {
            Kod = kod;
            Nazev = nazev;
        }

        public string Kod { get; set; }
        public string Nazev { get; set; }
    }
}
