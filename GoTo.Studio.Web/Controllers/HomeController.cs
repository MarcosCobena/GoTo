using GoTo.Studio.Web.Models;
using GoTo.Studio.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Ooui.AspNetCore;
using System.Diagnostics;
using Xamarin.Forms;

namespace GoTo.Studio.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var page = new IDEPage();

            return new ElementResult(page.GetOouiElement());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
