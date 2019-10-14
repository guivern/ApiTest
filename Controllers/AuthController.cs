using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene token de autenticación.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetToken()
        {
            var token = GenerateJwt();

            return Ok(token);
        }

        /// <summary>
        /// Genera un token utilizando el algoritmo SHA512.
        /// </summary>
        /// <returns></returns>
        private string GenerateJwt()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration.GetSection("Jwt:ExpireDays").Value)),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}