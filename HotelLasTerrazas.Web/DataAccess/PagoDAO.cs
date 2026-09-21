using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HotelLasTerrazas.Web.Models;

namespace HotelLasTerrazas.Web.DataAccess
{
    public class PagoDAO
    {
        /// <summary>
        /// Llama a sp_RegistrarPago. Registra el pago y genera el comprobante en una sola operación.
        /// </summary>
        public int Registrar(Pago pago)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("sp_RegistrarPago", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
                comando.Parameters.AddWithValue("@IdMetodoPago", pago.IdMetodoPago);
                comando.Parameters.AddWithValue("@IdTipoComprobante", pago.IdTipoComprobante);
                comando.Parameters.AddWithValue("@Serie", pago.Serie);
                comando.Parameters.AddWithValue("@Numero", pago.Numero);

                conexion.Open();
                object resultado = comando.ExecuteScalar();
                return Convert.ToInt32(resultado);
            }
        }

        public List<MetodoPago> ListarMetodosPago()
        {
            var lista = new List<MetodoPago>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("SELECT * FROM MetodoPago WHERE Estado = 1", conexion))
            {
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new MetodoPago
                        {
                            IdMetodoPago = Convert.ToInt32(lector["IdMetodoPago"]),
                            NombreMetodo = lector["NombreMetodo"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}