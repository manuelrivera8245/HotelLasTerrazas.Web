using System.Web.Optimization;

namespace HotelLasTerrazas.Web
{
    public class BundleConfig
    {
        // Por ahora usamos Bootstrap y jQuery vía CDN en _Layout.cshtml,
        // así que no hace falta registrar bundles locales todavía.
        // Cuando restaures los paquetes NuGet (bootstrap, jQuery) puedes
        // agregar aquí los bundles de Scripts/Content si prefieres servirlos localmente.
        public static void RegisterBundles(BundleCollection bundles)
        {
        }
    }
}
