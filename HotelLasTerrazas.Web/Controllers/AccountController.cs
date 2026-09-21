using HotelLasTerrazas.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;
using HotelLasTerrazas.Web.DataAccess;

namespace HotelLasTerrazas.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            Usuario usuario = usuarioDAO.ValidarCredenciales(modelo.NombreUsuario, modelo.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(modelo);
            }

            // Guardar datos de sesión
            Session["IdUsuario"] = usuario.IdUsuario;
            Session["NombreUsuario"] = usuario.NombreUsuario;
            Session["Rol"] = usuario.NombreRol;

            // Si es Cliente, también guardamos su IdCliente
            if (usuario.NombreRol == "Cliente")
            {
                int idCliente = usuarioDAO.ObtenerIdClientePorUsuario(usuario.IdUsuario);
                Session["IdCliente"] = idCliente;
                return RedirectToAction("Buscar", "Reserva");
            }

            if (usuario.NombreRol == "Recepcionista")
                return RedirectToAction("Index", "Recepcion");

            if (usuario.NombreRol == "Administrador")
                return RedirectToAction("Habitaciones", "Admin");

            return RedirectToAction("Login");
        }

        // GET: /Account/Registro
        public ActionResult Registro()
        {
            return View();
        }

        // POST: /Account/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Cliente modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            usuarioDAO.RegistrarCliente(modelo);

            TempData["Mensaje"] = "Cuenta creada correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
