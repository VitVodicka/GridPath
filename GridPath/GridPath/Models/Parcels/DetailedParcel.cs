namespace GridPath.Models.Parcels
{
    public class DetailedParcel
    {
        public DetailedParcel(string id, string typParcely, string druhCislovaniParcely, string kmenoveCisloParcely, string poddeleniCislaParcely, CadastralArea katastralniUzemi, string vymera, LV lv, DruhPozemku druhPozemku, string stavba, string pravoStavby, DefinicniBod definicniBod, string zpusobVyuziti, ZpusobyOchrany zpusobyOchrany, string rizeniPlomby)
        {
            Id = id;
            TypParcely = typParcely;
            DruhCislovaniParcely = druhCislovaniParcely;
            KmenoveCisloParcely = kmenoveCisloParcely;
            PoddeleniCislaParcely = poddeleniCislaParcely;
            KatastralniUzemi = katastralniUzemi;
            Vymera = vymera;
            Lv = lv;
            DruhPozemku = druhPozemku;
            Stavba = stavba;
            PravoStavby = pravoStavby;
            DefinicniBod = definicniBod;
            ZpusobVyuziti = zpusobVyuziti;
            ZpusobyOchrany = zpusobyOchrany;
            RizeniPlomby = rizeniPlomby;
        }

        public string Id { get; set; }
        public string TypParcely { get; set; }
        public string DruhCislovaniParcely { get; set; }
        public string KmenoveCisloParcely { get; set; }
        public string PoddeleniCislaParcely { get; set; }
        public CadastralArea KatastralniUzemi { get; set; }
        public string Vymera { get; set; }
        public LV Lv { get; set; }
        public DruhPozemku DruhPozemku { get; set; }
        public string Stavba { get; set; }
        public string PravoStavby { get; set; }
        public DefinicniBod DefinicniBod { get; set; }
        public string ZpusobVyuziti { get; set; }
        public ZpusobyOchrany ZpusobyOchrany { get; set; }
        public string RizeniPlomby { get; set; }
    }
}
