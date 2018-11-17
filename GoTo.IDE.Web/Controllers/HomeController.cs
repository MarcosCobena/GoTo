using GoTo.IDE.Web.Models;
using GoTo.IDE.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Ooui.AspNetCore;
using System.Diagnostics;
using Xamarin.Forms;

namespace GoTo.IDE.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var page = new IDEPage();

            return new ElementResult(page.GetOouiElement());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
