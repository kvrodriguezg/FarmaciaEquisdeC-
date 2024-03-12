using FarmaciaEquisde.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web.Providers.Entities;
using vistas.Controllers;

namespace FarmaciaEquisde.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult login()
        {
            Datos.rut = "";
            Datos.clave = "";
            Datos.nivel = 0;
            Datos.area = 0;
            return View("Views/Vistas/login.cshtml");
        }

        public IActionResult Index()
        {
            return View("/Views/Vistas/Index.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}