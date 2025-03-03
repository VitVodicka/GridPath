using GridPath.Helper;
using GridPath.Models;
using GridPath.Models.Parcels;
using GridPath.Models.PolygonParcels;
using GridPath.Services.ApiServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace GridPath.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ParcelService _parcelService;
       
        public static HashSet<Parcel> parcelsFromAPIPolygon = new HashSet<Parcel>();
        public static HashSet<DetailedParcel> parcelsParameters = new HashSet<DetailedParcel>();

        // Konstruktor pro injektování ParcelService
        public HomeController(ILogger<HomeController> logger, ParcelService parcelService)
        {
            _logger = logger;
            _parcelService = parcelService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string parcelData = await _parcelService.GetParcelFromParameters();
                // Pøedání dat do View nebo jejich další zpracování
                return View("Index", parcelData); // Nebo jiný zpùsob zobrazení dat
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi získávání dat o parcele.");
                return View("Error");
            }
        }
        public async Task<IActionResult> Neighbour()
        {
            try
            {
                string parcelData = await _parcelService.GetNeighbourParcels();
                // Pøedání dat do View nebo jejich další zpracování
                return View("Neighbour", parcelData); // Nebo jiný zpùsob zobrazení dat
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi získávání dat o parcele.");
                return View("Error");
            }
        }
        public async Task<IActionResult> ParcelId()
        {
            try
            {
                return  View("Parcel","data");
                //string parcelData = await _parcelService.GetParcelFromId();
                // Pøedání dat do View nebo jejich další zpracování
               // return View("ParcelId", parcelData); // Nebo jiný zpùsob zobrazení dat
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi získávání dat o parcele.");
                return View("Error");
            }
        }
        public async Task<IActionResult> Polygon()
        {
            try
            {
                string parcelData = await _parcelService.GetParcelsByPolygon
                    (CoordinateConversion.ConvertLineToRectangle(16.23, 49.29, 16.23, 49.28));

                // Pøedání dat do View nebo jejich další zpracování
                return View("Polygon", parcelData); // Nebo jiný zpùsob zobrazení dat
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi získávání dat o parcele.");
                return View("Error");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
