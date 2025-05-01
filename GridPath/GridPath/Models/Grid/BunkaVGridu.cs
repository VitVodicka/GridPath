using GridPath.Models.Parcels;

namespace GridPath.Models.Grid
{
    public class BunkaVGridu
    {
        public List<DetailRatedParcel> Pozemky { get; set; } = new List<DetailRatedParcel>();
        public double StredniHodnota { get; set; } = 0;

        public BunkaVGridu()
        {
            double soucet = 0;
            for (int i = 0; i < Pozemky.Count; i++)
            {
                soucet += Pozemky[i].Points;
            }
            if (Pozemky.Count > 0)
            {
                StredniHodnota = soucet / Pozemky.Count;
            }
            else
            {
                StredniHodnota = 0;
            }
        }
    }
}
