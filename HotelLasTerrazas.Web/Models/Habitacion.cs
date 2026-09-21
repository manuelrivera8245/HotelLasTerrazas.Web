using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelLasTerrazas.Web.Models
{
    public class Habitacion
    {
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public int Piso { get; set; }
        public int IdTipoHabitacion { get; set; }
        public string NombreTipo { get; set; }
        public int Capacidad { get; set; }
        public int IdEstadoHabitacion { get; set; }
        public string EstadoHabitacion { get; set; }
    }

    public class TipoHabitacion
    {
        public int IdTipoHabitacion { get; set; }
        public string NombreTipo { get; set; }
        public int Capacidad { get; set; }
        public string Descripcion { get; set; }
    }

    public class Temporada
    {
        public int IdTemporada { get; set; }
        public string NombreTemporada { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public int IdTipoHabitacion { get; set; }
        public int IdTemporada { get; set; }
        public decimal PrecioNoche { get; set; }
    }

    /// Resultado de la búsqueda de disponibilidad (SP: sp_BuscarDisponibilidad)
    public class HabitacionDisponible
    {
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public string NombreTipo { get; set; }
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
    }
}
