using GridPath.Models;
using GridPath.Services.ApiServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GridPath.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ParcelService _parcelService;

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
                string parcelData = await _parcelService.GetParcelFromId();
                // Pøedání dat do View nebo jejich další zpracování
                return View("ParcelId", parcelData); // Nebo jiný zpùsob zobrazení dat
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
