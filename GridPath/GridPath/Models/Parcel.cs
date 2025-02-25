namespace GridPath.Models
{
    public class Parcel
    {
        public int Id { get; set; }
        public string ParcelType { get; set; }
        public int NumberingType { get; set; }
        public int ParcelNumber { get; set; }
        public int? SubdivisionNumber { get; set; }
        public CadastralArea CadastralArea { get; set; }
    }
}
