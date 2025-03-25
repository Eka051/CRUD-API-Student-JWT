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
            StudentContext context = new StudentContext(__constr);
            Student student = context.GetAdminStudent(loginData.email, loginData.password);

            if (student == null || student.password != loginData.password)
            {
                return Unauthorized(new { message = "Email atau password salah" });
            }

            JwtHelper jwtHelper = new JwtHelper(_config);
            var token = jwtHelper.GenerateToken(student);

            return Ok(new
            {
                token = token,
                admin = new
                {
                    id = student.student_id,
                    name = student.name,
                    email = student.email,
                }
            });
        }

    }
}
