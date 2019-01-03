using GoTo.Studio.Web.Pages;
using Ooui;
using Xamarin.Forms;

namespace GoTo.Studio.WebAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            Forms.Init();

            var page = new IDEPage();

            UI.Publish("/", page.GetOouiElement());
        }
    }
}
