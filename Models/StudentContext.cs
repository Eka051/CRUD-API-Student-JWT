using Microsoft.AspNetCore.Mvc;
using Npgsql;
using CRUD_API_Student_JWT.Helpers;

namespace CRUD_API_Student_JWT.Models
{
    public class StudentContext
    {
        private string __constr;

        public StudentContext(string constr)
        {
            __constr = constr;
        }


        public List<Student> FindAll()
        {
            List<Student> students = new List<Student>();
            SqlDBHelper dbHelper = new SqlDBHelper(__constr);
            string query = "SELECT id_student, nim, nama, prodi, tanggal_lahir FROM students ORDER BY id_student";

            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        student_id = reader.GetInt32(0),
                        nim = reader.GetString(1),
                        name = reader.GetString(2),
                        prodi = reader.GetString(3),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(4))
                    });

                }
                cmd.Dispose();
                dbHelper.CloseConnection();
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
            return students;
        }

        public Student FindById(int student_id)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(__constr);
            string query = "SELECT id_student, nim, nama, prodi, tanggal_lahir FROM students WHERE id_student = @id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id", student_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Student student = new Student
                    {
                        student_id = reader.GetInt32(0),
                        nim = reader.GetString(1),
                        name = reader.GetString(2),
                        prodi = reader.GetString(3),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(4))
                    };
                    return student;
                }
                cmd.Dispose();
                dbHelper.CloseConnection();
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
            return null;
        }

        public Student Create([FromBody] Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(__constr);
            string query = @"INSERT INTO students (nim, nama, prodi, tanggal_lahir) VALUES (@nim, @nama, @prodi, @dateOfBirth) RETURNING *";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nim", student.nim);
                cmd.Parameters.AddWithValue("@nama", student.name);
                cmd.Parameters.AddWithValue("@prodi", student.prodi);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth);

                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student.student_id = reader.GetInt32(0);
                }
                cmd.Dispose();
                dbHelper.CloseConnection();
                return student;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }

        public Student Update(int student_id, [FromBody] Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(__constr);
            string query = @"UPDATE students 
                     SET nim = @nim, nama = @nama, prodi = @prodi, tanggal_lahir = @dateOfBirth
                     WHERE id_student = @id_student 
                     RETURNING *";

            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nim", student.nim);
                cmd.Parameters.AddWithValue("@nama", student.name);
                cmd.Parameters.AddWithValue("@prodi", student.prodi);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth.ToDateTime(TimeOnly.MinValue)); 
                cmd.Parameters.AddWithValue("@id_student", student_id);

                NpgsqlDataReader reader = cmd.ExecuteReader();
                Student updatedStudent = null;

                if (reader.Read())
                {
                    updatedStudent = new Student
                    {
                        student_id = reader.GetInt32(0),
                        nim = reader.GetString(1),
                        name = reader.GetString(2),
                        prodi = reader.GetString(3),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    };
                }

                cmd.Dispose();
                dbHelper.CloseConnection();

                if (updatedStudent == null)
                {
                    throw new Exception("Failed to retrieve updated student.");
                }

                return updatedStudent;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }


        public Student Delete(Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(__constr);
            string query = "DELETE FROM students WHERE id_student = @id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id", student.student_id);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                dbHelper.CloseConnection();
                return student;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }
    }
}
