namespace GridPath.Models.Parcels
{
    public class DetailRatedParcel
    {
        public DetailRatedParcel(DetailedParcel detailedParcel, double points)
        {
            DetailedParcel = detailedParcel;
            Points = points;
        }

        public DetailedParcel DetailedParcel { get; set; }
        public double Points { get; set; }
    }
}
