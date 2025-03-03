namespace GridPath.Models.Parcels
{
    public class DetailedParcel
    {
        public long Id { get; set; }
        public string TypParcely { get; set; }
        public int DruhCislovaniParcely { get; set; }
        public int KmenoveCisloParcely { get; set; }
        public int PoddeleniCislaParcely { get; set; }
        public CadastralArea KatastralniUzemi { get; set; }
        public double Vymera { get; set; }
        public LV Lv { get; set; }
        public DruhPozemku DruhPozemku { get; set; }
        public object Stavba { get; set; }
        public object PravoStavby { get; set; }
        public DefinicniBod DefinicniBod { get; set; }
        public object ZpusobVyuziti { get; set; }
        public List<ZpusobyOchrany> ZpusobyOchrany { get; set; }
        public List<object> RizeniPlomby { get; set; }
    }
}
