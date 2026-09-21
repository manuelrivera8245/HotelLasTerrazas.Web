using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelLasTerrazas.Web.DataAccess;
using HotelLasTerrazas.Web.Models;

namespace HotelLasTerrazas.Web.Controllers
{
    public class RecepcionController : Controller
    {
        private readonly ReservaDAO reservaDAO = new ReservaDAO();
        private readonly PagoDAO pagoDAO = new PagoDAO();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Recepcionista")
            {
                filterContext.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: /Recepcion/Index?estado=Pendiente
        public ActionResult Index(string estado)
        {
            ViewBag.EstadoFiltro = estado;
            var reservas = reservaDAO.ListarPorEstado(estado);
            return View(reservas);
        }

        [HttpPost]
        public ActionResult Confirmar(int idReserva)
        {
            reservaDAO.Confirmar(idReserva);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CheckIn(int idReserva)
        {
            reservaDAO.CheckIn(idReserva);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CheckOut(int idReserva)
        {
            reservaDAO.CheckOut(idReserva);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Cancelar(int idReserva, string motivo)
        {
            reservaDAO.Cancelar(idReserva, motivo);
            return RedirectToAction("Index");
        }

        // GET: /Recepcion/RegistrarPago?idReserva=145
        public ActionResult RegistrarPago(int idReserva)
        {
            ViewBag.IdReserva = idReserva;
            ViewBag.MetodosPago = pagoDAO.ListarMetodosPago();
            return View();
        }

        // POST: /Recepcion/RegistrarPago
        [HttpPost]
        public ActionResult RegistrarPago(Pago modelo)
        {
            pagoDAO.Registrar(modelo);
            TempData["Mensaje"] = "Pago registrado y comprobante emitido correctamente.";
            return RedirectToAction("Index");
        }
    }
}
