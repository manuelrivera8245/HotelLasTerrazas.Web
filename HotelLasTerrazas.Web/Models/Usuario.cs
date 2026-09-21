using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelLasTerrazas.Web.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ClaveHash { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
    }

    public class Cliente
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }

        // Formulario de registro
        public string Password { get; set; }
    }

    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public int IdUsuario { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Cargo { get; set; }
        public DateTime FechaIngreso { get; set; }
    }

    public class MetodoPago
    {
        public int IdMetodoPago { get; set; }
        public string NombreMetodo { get; set; }
    }

    public class Pago
    {
        public int IdPago { get; set; }
        public int IdReserva { get; set; }
        public int IdMetodoPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }

        // Formulario de registro de pago + comprobante
        public int IdTipoComprobante { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
    }

    /// Modelo simple para el formulario de login
    public class LoginModel
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
    }
}
