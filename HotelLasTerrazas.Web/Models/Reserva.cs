using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelLasTerrazas.Web.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int IdEstadoReserva { get; set; }
        public string Estado { get; set; }
        public decimal MontoTotal { get; set; }
        public string MotivoCancelacion { get; set; }

        // Formulario de creación de reserva
        public List<int> HabitacionesSeleccionadas { get; set; } = new List<int>();
    }

    /// Fila que devuelve la vista vw_ReservasDetalle
    public class ReservaDetalle
    {
        public int IdReserva { get; set; }
        public string Cliente { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int Noches { get; set; }
        public string Estado { get; set; }
        public decimal MontoTotal { get; set; }
        public string Habitaciones { get; set; }
    }

    public class DetalleReserva
    {
        public int IdDetalleReserva { get; set; }
        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }
        public decimal PrecioNocheAplicado { get; set; }
    }
}
