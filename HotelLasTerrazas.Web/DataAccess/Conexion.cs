using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelLasTerrazas.Web.DataAccess
{
    /// <summary>
    /// Provee una conexión SqlConnection lista para usar,
    /// leyendo la cadena de conexión desde Web.config.
    /// </summary>
    public static class ConexionBD
    {
        private static readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["HotelConnection"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}