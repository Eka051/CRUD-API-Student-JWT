using Microsoft.AspNetCore.Mvc;
using Npgsql;
using CRUD_API_Student_JWT.Helpers;

namespace CRUD_API_Student_JWT.Models
{
    public class StudentContext
    {
        private string constr;

        public StudentContext(string constr)
        {
            this.constr = constr;
        }

        public List<Student> FindAll()
        {
            List<Student> students = new List<Student>();
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "SELECT * FROM students";

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
                        email = reader.GetString(3),
                        prodi = reader.GetString(4),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(5))
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
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "SELECT student_id, nim, name, email, prodi, date_of_birth FROM students WHERE student_id = @id";
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
                        email = reader.GetString(3),
                        prodi = reader.GetString(4),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(5))
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
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "INSERT INTO students (nim, name, email, prodi, date_of_birth) VALUES (@nim, @name, @email, @prodi, @dateOfBirth) RETURNING student_id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nim", student.nim);
                cmd.Parameters.AddWithValue("@name", student.name);
                cmd.Parameters.AddWithValue("@email", student.email);
                cmd.Parameters.AddWithValue("@prodi", student.prodi);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    student.student_id = (int)result;
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
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = @"UPDATE students 
                     SET nim = @nim, name = @name, email = @email, prodi = @prodi, date_of_birth = @dateOfBirth 
                     WHERE student_id = @student_id 
                     RETURNING *";

            try
            {
                NpgsqlCommand cmd = dbHelper.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nim", student.nim);
                cmd.Parameters.AddWithValue("@name", student.name);
                cmd.Parameters.AddWithValue("@email", student.email);
                cmd.Parameters.AddWithValue("@prodi", student.prodi);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth.ToDateTime(TimeOnly.MinValue)); 
                cmd.Parameters.AddWithValue("@student_id", student_id);

                NpgsqlDataReader reader = cmd.ExecuteReader();
                Student updatedStudent = null;

                if (reader.Read())
                {
                    updatedStudent = new Student
                    {
                        student_id = reader.GetInt32(0),
                        nim = reader.GetString(1),
                        name = reader.GetString(2),
                        email = reader.GetString(3),
                        prodi = reader.GetString(4),
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(5))
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
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "DELETE FROM students WHERE student_id = @id";
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
