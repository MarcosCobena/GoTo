using Ooui;
using Xamarin.Forms;

namespace GoTo.Studio
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
