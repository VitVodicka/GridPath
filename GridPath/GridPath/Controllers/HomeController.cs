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

        public static List<Parcel> parcelsFromBeginningAndEndPoint = new List<Parcel>();
        public static int numberOfPossibleCalling;



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
                _parcelService.ReturnNumberOfPossibleCalling();
                string parcelData = await _parcelService.GetParcelFromParameters();
                return View("Index", parcelData); 
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
                return View("Neighbour", parcelData);
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
                DetailedParcel parcel = await _parcelService.GetParcelFromId("459379744");
                return  View("ParcelId",parcel);

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
                
                return View("Polygon", parcelData); 
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
