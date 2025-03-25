using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using CRUD_API_Student_JWT.Helpers;
using CRUD_API_Student_JWT.Models;
using Microsoft.AspNetCore.Authorization;

namespace Student_CRUD.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly string _constr;

        public StudentController(IConfiguration configuration)
        {
            _constr = configuration.GetConnectionString("WebApiDatabase");
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<Student>> FindAll()
        {
            var context = new StudentContext(_constr);
            var students = context.FindAll();
            if (students == null)
            {
                return NotFound("No student data found");
            }
            return Ok(students);
        }

        [HttpGet("{student_id}")]
        public ActionResult<Student> FindById(int student_id)
        {
            var context = new StudentContext(_constr);
            var student = context.FindById(student_id);
            if (student == null)
            {
                return NotFound($"Student with ID {student_id} Not Found");
            }
            return Ok(student);
        }

        [HttpPost("create"), Authorize]
        public ActionResult<Student> Create([FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest("Error when insert data");
            }

            var context = new StudentContext(_constr);
            var createdStudent = context.Create(student);
            return Ok(createdStudent);
        }

        [HttpPut("update/{student_id}"), Authorize]
        public ActionResult<Student> Update(int student_id, [FromBody] Student student)
        {

            var context = new StudentContext(_constr);
            var existingStudent = context.FindById(student_id);
            if (student_id <= 0 || student == null)
            {
                return BadRequest("Invalid student data or ID");
            }
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {student_id} not found");
            }
            if (student.student_id != 0 && student.student_id != student_id)
            {
                return BadRequest("Mismatch between student ID in URL and request body.");
            }
            var updatedStudent = context.Update(student_id, student);
            return Ok(updatedStudent);
        }

        [HttpDelete("delete/{student_id}"), Authorize]
        public ActionResult Delete(int student_id)
        {
            if (student_id <= 0)
            {
                return BadRequest("Invalid Student ID");
            }

            var context = new StudentContext(_constr);
            var student = context.FindById(student_id);
            if (student == null)
            {
                return NotFound($"Student with ID {student_id} Not Found");
            }

            context.Delete(student);
            return NoContent();
        }
    }
}
