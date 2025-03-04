namespace GridPath.Models.Parcels
{
    public class LV
    {
        public LV(string id, string cislo, CadastralArea katastralniUzemi)
        {
            Id = id;
            Cislo = cislo;
            KatastralniUzemi = katastralniUzemi;
        }

        public string Id { get; set; }
        public string Cislo { get; set; }
        public CadastralArea KatastralniUzemi { get; set; }
    }
}
