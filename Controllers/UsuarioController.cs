using Microsoft.AspNetCore.Mvc;
using VIANDRIVEFULL.Models;


namespace VIANDRIVEFULL.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Dashboard()
        {
            if (TempData["UsuarioId"] == null)
                return RedirectToAction("Login", "Auth");

            var modelo = new UsuarioDashboardViewModel
            {
                Id = Convert.ToInt32(TempData["UsuarioId"]),
                Nombre = TempData["UsuarioNombre"]?.ToString(),
                Email = TempData["UsuarioEmail"]?.ToString(),
                Rol = TempData["UsuarioRol"]?.ToString()
            };

            return View(modelo);
        }
    }
}
