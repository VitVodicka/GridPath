namespace GridPath.Models.Parcels
{
    public class DetailRatedParcel
    {
        public DetailRatedParcel(DetailedParcel detailedParcel, double points, string warning)
        {
            DetailedParcel = detailedParcel;
            Points = points;
            Warning = warning;
        }

        public DetailedParcel DetailedParcel { get; set; }
        public double Points { get; set; }
        public string Warning { get; set; }
    }
}
