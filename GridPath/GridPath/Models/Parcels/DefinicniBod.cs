namespace GridPath.Models.Parcels
{
    public class DefinicniBod
    {
        public DefinicniBod(string id, string x, string y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public string Id { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
    }
}
