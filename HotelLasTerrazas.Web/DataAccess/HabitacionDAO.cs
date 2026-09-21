using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HotelLasTerrazas.Web.Models;

namespace HotelLasTerrazas.Web.DataAccess
{
    public class HabitacionDAO
    {
        /// <summary>
        /// Llama a sp_BuscarDisponibilidad. idTipoHabitacion puede ser null (todas).
        /// </summary>
        public List<HabitacionDisponible> BuscarDisponibilidad(DateTime fechaEntrada, DateTime fechaSalida, int? idTipoHabitacion)
        {
            var lista = new List<HabitacionDisponible>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("sp_BuscarDisponibilidad", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@FechaEntrada", fechaEntrada);
                comando.Parameters.AddWithValue("@FechaSalida", fechaSalida);
                comando.Parameters.AddWithValue("@IdTipoHabitacion", (object)idTipoHabitacion ?? DBNull.Value);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new HabitacionDisponible
                        {
                            IdHabitacion = Convert.ToInt32(lector["IdHabitacion"]),
                            NumeroHabitacion = lector["NumeroHabitacion"].ToString(),
                            NombreTipo = lector["NombreTipo"].ToString(),
                            Capacidad = Convert.ToInt32(lector["Capacidad"]),
                            PrecioNoche = Convert.ToDecimal(lector["PrecioNoche"])
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Lista todas las habitaciones (vista vw_Habitaciones) - uso en el panel Admin.
        /// </summary>
        public List<Habitacion> ListarTodas()
        {
            var lista = new List<Habitacion>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("SELECT * FROM vw_Habitaciones ORDER BY NumeroHabitacion", conexion))
            {
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Habitacion
                        {
                            IdHabitacion = Convert.ToInt32(lector["IdHabitacion"]),
                            NumeroHabitacion = lector["NumeroHabitacion"].ToString(),
                            Piso = Convert.ToInt32(lector["Piso"]),
                            NombreTipo = lector["NombreTipo"].ToString(),
                            Capacidad = Convert.ToInt32(lector["Capacidad"]),
                            EstadoHabitacion = lector["EstadoHabitacion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Lista los tipos de habitación (para combos en formularios).
        /// </summary>
        public List<TipoHabitacion> ListarTipos()
        {
            var lista = new List<TipoHabitacion>();

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand("SELECT * FROM TipoHabitacion WHERE Estado = 1", conexion))
            {
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new TipoHabitacion
                        {
                            IdTipoHabitacion = Convert.ToInt32(lector["IdTipoHabitacion"]),
                            NombreTipo = lector["NombreTipo"].ToString(),
                            Capacidad = Convert.ToInt32(lector["Capacidad"]),
                            Descripcion = lector["Descripcion"] as string
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Registra una nueva habitación (INSERT directo, uso administrativo).
        /// </summary>
        public void Registrar(Habitacion habitacion)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand(
                "INSERT INTO Habitacion (NumeroHabitacion, Piso, IdTipoHabitacion, IdEstadoHabitacion) " +
                "VALUES (@NumeroHabitacion, @Piso, @IdTipoHabitacion, 1)", conexion))
            {
                comando.Parameters.AddWithValue("@NumeroHabitacion", habitacion.NumeroHabitacion);
                comando.Parameters.AddWithValue("@Piso", habitacion.Piso);
                comando.Parameters.AddWithValue("@IdTipoHabitacion", habitacion.IdTipoHabitacion);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
