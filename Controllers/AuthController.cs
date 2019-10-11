using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiTest.Data;
using ApiTest.Models;
using ApiTest.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private DataContext _context;
        private IConfiguration _configuration;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            AddDefaultUser();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto credenciales)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username.Equals(credenciales.Username));

            if (usuario == null)
                return Unauthorized();

            if (!IsValidPassword(credenciales.Password, usuario))
                return Unauthorized();

            var token = GenerateToken(usuario);

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Si no hay usuarios en la bd, crea un usuario predeterminado.
        /// </summary>
        /// <returns></returns>
        private void AddDefaultUser()
        {
            if (_context.Usuarios.Any()) return;

            var password = "password";
            var passwordSalt = GenerateSalt();
            var PasswordHash = GenerateHash(password, passwordSalt);

            var usuario = new Usuario()
            {
                Username = "invitado",
                PasswordSalt = passwordSalt,
                PasswordHash = PasswordHash,
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        /// <summary>
        /// Verifica si el password proporcionado corresponde al password del usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsValidPassword(string password, Usuario usuario)
        {
            using (var hmac = new HMACSHA512(usuario.PasswordSalt))
            {
                // genera un hash a partir del password y el passwordSalt
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // compara el hash generado con el hash original 
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != usuario.PasswordHash[i])
                        return false;
                }
                return true;
            }
        }

        private byte[] GenerateSalt()
        {
            using (var hmac = new HMACSHA512())
            {
                return hmac.Key; // clave para generar el hash
            }
        }

        private byte[] GenerateHash(string password, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Genera un token utilizando el algoritmo SHA512
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private string GenerateToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration.GetSection("Jwt:ExpireDays").Value)),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}