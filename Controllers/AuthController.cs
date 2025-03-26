using CRUD_API_Student_JWT.Helper;
using CRUD_API_Student_JWT.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRUD_API_Student_JWT.Controllers
{
    public class AuthController : Controller
    {
        private readonly string __constr;
        private readonly IConfiguration _config;
        public IActionResult Index()
        {
            return View();
        }

        public AuthController(IConfiguration config)
        {
            _config = config;
            __constr = _config.GetConnectionString("WebApiDatabase");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login loginData)
        {
            PersonContext context = new PersonContext(__constr);
            Person person = context.GetPersonByEmail(loginData.email);

            if (person == null || person.Password != loginData.password)
            {
                return Unauthorized(new { message = "Hanya admin yang memiliki izin untuk login" });
            }

            JwtHelper jwtHelper = new JwtHelper(_config);
            var token = jwtHelper.GenerateToken(person);

            return Ok(new
            {
                token = token,
                admin = new
                {
                    id = person.Id_person,
                    name = person.Nama,
                    email = person.Email,
                }
            });
        }

    }
}
