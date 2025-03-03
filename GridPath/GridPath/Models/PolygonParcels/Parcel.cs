namespace GridPath.Models.PolygonParcels
{
    public class Parcel
    {
        public Parcel(string id, string typParcely, string druhCislovaniParcely, string kmenoveCisloParcely, string poddeleniCislaParcely, CadastralArea katastralniUzemi)
        {
            Id = id;
            TypParcely = typParcely;
            DruhCislovaniParcely = druhCislovaniParcely;
            KmenoveCisloParcely = kmenoveCisloParcely;
            PoddeleniCislaParcely = poddeleniCislaParcely;
            KatastralniUzemi = katastralniUzemi;
        }

        public string Id { get; set; }
        public string TypParcely { get; set; }
        public string DruhCislovaniParcely { get; set; }
        public string KmenoveCisloParcely { get; set; }
        public string PoddeleniCislaParcely { get; set; }
        public CadastralArea KatastralniUzemi { get; set; }
    }
}
