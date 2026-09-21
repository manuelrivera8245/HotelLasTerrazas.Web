using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelLasTerrazas.Web.DataAccess;
using HotelLasTerrazas.Web.Models;

namespace HotelLasTerrazas.Web.Controllers
{
    /// Controlador usado por el rol Cliente para buscar disponibilidad y reservar.
    public class ReservaController : Controller
    {
        private readonly HabitacionDAO habitacionDAO = new HabitacionDAO();
        private readonly ReservaDAO reservaDAO = new ReservaDAO();

        /// Verifica que haya una sesión de Cliente activa antes de cada acción.
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["IdCliente"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: /Reserva/Buscar
        public ActionResult Buscar()
        {
            ViewBag.Tipos = habitacionDAO.ListarTipos();
            return View();
        }

        // POST: /Reserva/Buscar  (procesa el formulario de búsqueda)
        [HttpPost]
        public ActionResult Buscar(DateTime fechaEntrada, DateTime fechaSalida, int? idTipoHabitacion)
        {
            if (fechaSalida <= fechaEntrada)
            {
                ModelState.AddModelError("", "La fecha de salida debe ser posterior a la fecha de entrada.");
                ViewBag.Tipos = habitacionDAO.ListarTipos();
                return View();
            }

            var disponibles = habitacionDAO.BuscarDisponibilidad(fechaEntrada, fechaSalida, idTipoHabitacion);

            // Guardamos las fechas en sesión temporal para el paso de confirmación
            Session["Res_FechaEntrada"] = fechaEntrada;
            Session["Res_FechaSalida"] = fechaSalida;

            return View("Resultados", disponibles);
        }

        // POST: /Reserva/Confirmar  (el cliente eligió habitaciones y confirma)
        [HttpPost]
        public ActionResult Confirmar(int[] habitacionesSeleccionadas)
        {
            if (habitacionesSeleccionadas == null || habitacionesSeleccionadas.Length == 0)
            {
                TempData["Error"] = "Debes seleccionar al menos una habitación.";
                return RedirectToAction("Buscar");
            }

            int idCliente = Convert.ToInt32(Session["IdCliente"]);
            DateTime fechaEntrada = (DateTime)Session["Res_FechaEntrada"];
            DateTime fechaSalida = (DateTime)Session["Res_FechaSalida"];

            var resultado = reservaDAO.Registrar(idCliente, fechaEntrada, fechaSalida,
                new System.Collections.Generic.List<int>(habitacionesSeleccionadas));

            ViewBag.IdReserva = resultado.IdReserva;
            ViewBag.MontoTotal = resultado.MontoTotal;
            ViewBag.FechaEntrada = fechaEntrada;
            ViewBag.FechaSalida = fechaSalida;

            return View("Confirmacion");
        }

        // GET: /Reserva/MisReservas
        public ActionResult MisReservas()
        {
            int idCliente = Convert.ToInt32(Session["IdCliente"]);
            var reservas = reservaDAO.ListarPorCliente(idCliente);
            return View(reservas);
        }

        // POST: /Reserva/Cancelar
        [HttpPost]
        public ActionResult Cancelar(int idReserva)
        {
            reservaDAO.Cancelar(idReserva, "Cancelada por el cliente");
            return RedirectToAction("MisReservas");
        }
    }
}
