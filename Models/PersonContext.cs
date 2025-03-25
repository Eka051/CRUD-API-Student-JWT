using CRUD_API_Student_JWT.Helper;
using CRUD_API_Student_JWT.Helpers;
using Npgsql;
using System.Data.SqlTypes;

namespace CRUD_API_Student_JWT.Models
{
    public class PersonContext
    {
        private string __constr;
        private string __errorMsqg;

        public PersonContext(string pObs) 
        {
            __constr = pObs;
        }
       
        public List<Person> ListPerson()
        {
            //inisiasi list kosong 
            List<Person> list1 = new List<Person>();

            string query = string.Format(@"SELECT * FROM person");
            SqlDBHelper db = new SqlDBHelper(this.__constr);

            try { 
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list1.Add(new Person()
                    {
                        Id_person = int.Parse(reader["id_person"].ToString()),
                        Nama = reader["nama"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                        Email = reader["email"].ToString(),
                        Password = reader["password"].ToString()
                    });
                }
                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex) 
            { 
                __errorMsqg = ex.Message;
            }

            return list1;
        }
        public Person GetPersonByEmail(string email)
        {
            Person person = null;

            string query = string.Format(@"SELECT * FROM person WHERE email = @email");
            SqlDBHelper db = new SqlDBHelper(this.__constr);

            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@email", email);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    person = new Person()
                    {
                        Id_person = int.Parse(reader["id_person"].ToString()),
                        Nama = reader["nama"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                        Email = reader["email"].ToString(),
                        Password = reader["password"].ToString()
                    };
                }
                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsqg = ex.Message;
            }

            return person;
        }

        public bool RegisterPerson(Person person)
        {
            bool result = false;
            string query = string.Format(@"INSERT INTO person (nama, alamat, email, password) 
                                  VALUES (@nama, @alamat, @email, @password)");
            SqlDBHelper db = new SqlDBHelper(this.__constr);

            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nama", person.Nama);
                cmd.Parameters.AddWithValue("@alamat", person.Alamat);
                cmd.Parameters.AddWithValue("@email", person.Email);
                cmd.Parameters.AddWithValue("@password", person.Password);

                int rowsAffected = cmd.ExecuteNonQuery();
                result = rowsAffected > 0;

                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsqg = ex.Message;
                Console.WriteLine("Error in RegisterPerson: " + ex.Message);
            }

            return result;
        }

    }
}
