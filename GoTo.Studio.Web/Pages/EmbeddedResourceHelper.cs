using System.IO;
using System.Reflection;

namespace GoTo.Studio.Web.Pages
{
    static class EmbeddedResourceHelper
    {
        internal static string Load(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"GoTo.Studio.Web.Pages.{fileName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
