using GridPath.Models.Parcels;

namespace GridPath.Models.Grid
{
    public class BunkaVGridu
    {
        public List<DetailRatedParcel> Pozemky { get; set; } = new List<DetailRatedParcel>();
        public double StredniHodnota { get; set; } = 0;

        public BunkaVGridu()
        {
        }
        public void UpdateStredniHodnota()
        {
            StredniHodnota = Pozemky.Count > 0 ? Pozemky.Average(p => p.Points) : 0;
        }
    }
}
