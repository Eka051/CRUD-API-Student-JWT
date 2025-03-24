namespace CRUD_API_Student_JWT.Models
{
    public class Student
    {
        public int student_id { get; set; }
        public string nim { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string prodi { get; set; }
        public DateOnly dateOfBirth { get; set; }

    }
}
