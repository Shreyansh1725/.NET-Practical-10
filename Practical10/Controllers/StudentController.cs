
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Practical10.Model;
using System.Data;

namespace Practical10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly string cs;

        public StudentController(IConfiguration configuration)
        {
            cs = configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET: api/Student
        // Get all students
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT Id, Name, Branch, Class FROM Student";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Student s = new Student();

                    s.Id = Convert.ToInt32(dr["Id"]);
                    s.Name = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString()!;
                    s.Branch = dr["Branch"] == DBNull.Value ? "" : dr["Branch"].ToString()!;
                    s.Class = dr["Class"] == DBNull.Value ? null : dr["Class"].ToString();

                    students.Add(s);
                }

                dr.Close();
            }

            return Ok(students);
        }

        // GET: api/Student/1
        // Get student by ID
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            Student student = new Student();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT id, Name, branch, class FROM Student WHERE id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    student.Id = Convert.ToInt32(dr["id"]);
                    student.Name = dr["Name"]?.ToString() ?? "";
                    student.Branch = dr["branch"]?.ToString() ?? "";
                    student.Class = dr["class"] == DBNull.Value
                        ? null
                        : dr["class"].ToString();
                }
                else
                {
                    return NotFound("Student not found");
                }
            }

            return Ok(student);
        }

        // POST: api/Student
        // Insert new student
        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Student (Name, branch, class)
                 VALUES (@Name, @Branch, @Class)"; 

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Branch", student.Branch);
                cmd.Parameters.AddWithValue("@Class", (object?)student.Class ?? DBNull.Value);

                con.Open();

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return Ok("Student added successfully");
                }
            }

            return BadRequest("Unable to add student");
        }

        // PUT: api/Student/1
        // Update existing student
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Student
                                 SET Name = @Name,
                                     Branch = @Branch,
                                     Class = @Class
                                 WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Branch", student.Branch);
                cmd.Parameters.AddWithValue("@Class", (object?)student.Class ?? DBNull.Value);

                con.Open();

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return Ok("Student updated successfully");
                }
            }

            return NotFound("Student not found");
        }

        // DELETE: api/Student/1
        // Delete student by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM Student WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    return Ok("Student deleted successfully");
                }
            }

            return NotFound("Student not found");
        }
    }
}
