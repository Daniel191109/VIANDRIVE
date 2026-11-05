using Microsoft.AspNetCore.Mvc;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models.DTOs;
using BCrypt.Net;

namespace VIANDRIVEFULL.Controllers
{
    public class AuthPanelController : Controller
    {
        private readonly AppDbContext _context;

        public AuthPanelController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            var usuarioDB = _context.Usuarios.FirstOrDefault(u => u.Email == dto.Email);

            if (usuarioDB == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuarioDB.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(dto);
            }

            TempData["UsuarioId"] = usuarioDB.Id;
            TempData["UsuarioNombre"] = usuarioDB.Nombre;
            TempData["UsuarioEmail"] = usuarioDB.Email;
            TempData["UsuarioRol"] = usuarioDB.Rol;

            return RedirectToAction("Dashboard", "Usuario");
        }
    }
}
