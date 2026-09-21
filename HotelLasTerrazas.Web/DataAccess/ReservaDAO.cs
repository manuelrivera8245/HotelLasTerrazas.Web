using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HotelLasTerrazas.Web.Models;

namespace HotelLasTerrazas.Web.DataAccess
{
    public class ReservaDAO
    {
        /// Registra una reserva completa llamando a sp_RegistrarReserva.
        /// Devuelve el Id de la reserva generada y el monto total calculado por el SP.
        public (int IdReserva, decimal MontoTotal) Registrar(int idCliente, DateTime fechaEntrada, DateTime fechaSalida, List<int> idsHabitaciones)
        {
            // El SP espera los ids separados por coma: "1,3,5"
            string habitacionesCsv = string.Join(",", idsHabitaciones);

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("sp_RegistrarReserva", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdCliente", idCliente);
                comando.Parameters.AddWithValue("@FechaEntrada", fechaEntrada);
                comando.Parameters.AddWithValue("@FechaSalida", fechaSalida);
                comando.Parameters.AddWithValue("@Habitaciones", habitacionesCsv);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        int idReserva = Convert.ToInt32(lector["IdReservaGenerada"]);
                        decimal monto = Convert.ToDecimal(lector["MontoTotal"]);
                        return (idReserva, monto);
                    }
                }
            }

            throw new Exception("No se pudo registrar la reserva.");
        }

        /// Reservas de un cliente específico (para "Mis reservas").
        public List<ReservaDetalle> ListarPorCliente(int idCliente)
        {
            var lista = new List<ReservaDetalle>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand(
                "SELECT rd.* FROM vw_ReservasDetalle rd " +
                "INNER JOIN Reserva r ON r.IdReserva = rd.IdReserva " +
                "WHERE r.IdCliente = @IdCliente ORDER BY rd.FechaReserva DESC", conexion))
            {
                comando.Parameters.AddWithValue("@IdCliente", idCliente);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(MapearReservaDetalle(lector));
                    }
                }
            }
            return lista;
        }

        /// Llama a sp_ReservasPorEstado. nombreEstado puede ser null (todas).
        /// Uso: panel de Recepción.
        public List<ReservaDetalle> ListarPorEstado(string nombreEstado)
        {
            var lista = new List<ReservaDetalle>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("sp_ReservasPorEstado", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@NombreEstado", (object)nombreEstado ?? DBNull.Value);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(MapearReservaDetalle(lector));
                    }
                }
            }
            return lista;
        }

        public void Confirmar(int idReserva)
        {
            EjecutarSpSimple("sp_ConfirmarReserva", idReserva);
        }

        public void CheckIn(int idReserva)
        {
            EjecutarSpSimple("sp_CheckIn", idReserva);
        }

        public void CheckOut(int idReserva)
        {
            EjecutarSpSimple("sp_CheckOut", idReserva);
        }

        public void Cancelar(int idReserva, string motivo)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("sp_CancelarReserva", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdReserva", idReserva);
                comando.Parameters.AddWithValue("@Motivo", (object)motivo ?? DBNull.Value);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        //  Helpers privados 

        private void EjecutarSpSimple(string nombreSp, int idReserva)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand(nombreSp, conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdReserva", idReserva);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private ReservaDetalle MapearReservaDetalle(SqlDataReader lector)
        {
            return new ReservaDetalle
            {
                IdReserva = Convert.ToInt32(lector["IdReserva"]),
                Cliente = lector["Cliente"].ToString(),
                NumeroDocumento = lector["NumeroDocumento"].ToString(),
                FechaReserva = Convert.ToDateTime(lector["FechaReserva"]),
                FechaEntrada = Convert.ToDateTime(lector["FechaEntrada"]),
                FechaSalida = Convert.ToDateTime(lector["FechaSalida"]),
                Noches = Convert.ToInt32(lector["Noches"]),
                Estado = lector["Estado"].ToString(),
                MontoTotal = Convert.ToDecimal(lector["MontoTotal"]),
                Habitaciones = lector["Habitaciones"].ToString()
            };
        }
    }
}
