using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace User_prueba
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
    public class JwtService : IJwtService
    {

        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        new Claim("Id", user.Id.ToString()),
        new Claim("Name", user.Name),
        new Claim("Email", user.Email),
        new Claim("Image", user.Image),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],             // 🔁 Añade si validas issuer
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static (bool success, string message, int? userId) ValidarToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity == null || !identity.Claims.Any())
                    return (false, "No se encontraron claims en el token", null);

                var idClaim = identity.Claims.FirstOrDefault(x => x.Type == "Id");
                if (idClaim == null)
                    return (false, "No se encontró el claim 'Id' en el token", null);

                if (!int.TryParse(idClaim.Value, out int userId))
                    return (false, "El claim 'Id' no es un número válido", null);

                return (true, "Token válido", userId);
            }
            catch (Exception ex)
            {
                return (false, "Excepción: " + ex.Message, null);
            }
        }
        public static int? GetUserIdFromClaims(ClaimsIdentity identity)
        {
            if (identity == null || !identity.Claims.Any())
                return null;
            var idClaim = identity.Claims.FirstOrDefault(x => x.Type == "Id");
            if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
                return null;
            return userId;
        }

    }
}  
