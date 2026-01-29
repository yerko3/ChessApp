using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace User_prueba.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IRepository _database;
        private readonly ILogger<UserController> _logger; // es un sistema de logging de ASP.NET Core
        private readonly IJwtService _jwtService;

        public UserController(IRepository database, ILogger<UserController> logger, IJwtService jwtService)
        {
            _database = database;
            _logger = logger; //Esto es de C#
            _jwtService = jwtService;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _database.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { error = "Usuario no encontrado." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        // POST: user/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                    return BadRequest("Faltan datos obligatorios.");

                await _database.InsertUserAsync(user);
                return Ok( new { message = "Usuario creado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario.");
                return StatusCode(500, "Error interno al crear el usuario.");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(Request.LoginUser.Request login)
        {
            try
            {
                if(login == null)
                    return BadRequest("Login null.");
                if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                    return BadRequest("Faltan datos obligatorios.");
                var result = await _database.GetUserByEmailAsync(login.Email);
                var passwordShadow = User_prueba.Utils.Utils.ComputeSha256Hash(login.Password);
                if (result == null || result.Password != passwordShadow)
                    return Unauthorized("Credenciales inválidas");
                
                var token = _jwtService.GenerateToken(result);
                if (string.IsNullOrWhiteSpace(token))
                    return StatusCode(500, "Error al generar el token.");

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Asegúrate de que tu aplicación esté en HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1) // Ajusta la expiración según tus necesidades
                });
                //_logger.LogInformation("Usuario {Email} ha iniciado sesión.", login.Email);
                return Ok(new { message = "Login exitoso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión.");
                return StatusCode(500, "Error interno al iniciar sesión.");
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var (success, message, userId) = JwtService.ValidarToken(identity);

                if (!success || userId == null)
                    return Unauthorized(new { error = message });

                var user = await _database.GetUserByIdAsync(userId.Value);

                if (user == null)
                    return NotFound(new { error = "Usuario no encontrado." });

                return Ok(new { user.Id, user.Name, user.Email, user.Image });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario.");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
        //en point temporal este es de prueba ojo 
        [HttpGet("cookies")]
        public IActionResult GetCookies()
        {
            try
            {
                var cookies = Request.Cookies["jwt"];
                return Ok(cookies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las cookies.");
                return StatusCode(500, "Error interno al obtener las cookies.");
            }
        }

    }
}
