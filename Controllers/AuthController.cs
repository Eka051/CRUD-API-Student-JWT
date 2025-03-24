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
            __constr = _config.GetConnectionString("koneksi");
        }


        //[HttpPost("register")]
        //public IActionResult Register([FromBody] Person registerData)
        //{
            
        //    if (string.IsNullOrEmpty(registerData.Email) || string.IsNullOrEmpty(registerData.Password))
        //    {
        //        return BadRequest(new { message = "Email dan password harus diisi" });
        //    }

        //    PersonContext context = new PersonContext(__constr);

           
        //    Person existingPerson = context.GetPersonByEmail(registerData.Email);
        //    if (existingPerson != null)
        //    {
        //        return BadRequest(new { message = "Email sudah terdaftar" });
        //    }

            
        //    bool isRegistered = context.(registerData);

        //    if (isRegistered)
        //    {
        //        return Ok(new { message = "Registrasi berhasil" });
        //    }
        //    else
        //    {
        //        return StatusCode(500, new { message = "Registrasi gagal" });
        //    }
        //}



        [HttpPost("login")]
        public IActionResult Login([FromBody] Login loginData)
        {
            PersonContext context = new PersonContext(__constr);
            Person person = context.GetPersonByEmail(loginData.Email);

            if (person == null || person.Password != loginData.Password)
            {
                return Unauthorized(new { message = "Email atau password salah" });
            }

            JwtHelper jwtHelper = new JwtHelper(_config);
            var token = jwtHelper.GenerateToken(person);

            return Ok(new
            {
                token = token,
                person = new
                {
                    id = person.Id_person,
                    name = person.Nama,
                    email = person.Email
                }
            });
        }

    }
}
