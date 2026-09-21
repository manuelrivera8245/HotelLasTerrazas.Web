using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace HotelLasTerrazas.Web.DataAccess
{
    /// Provee una conexión SqlConnection lista para usar,
    /// leyendo la cadena de conexión desde Web.config.
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