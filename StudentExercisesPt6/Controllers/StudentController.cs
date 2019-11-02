using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesPt6.Models;

namespace StudentExercisesPt6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string q, string include)
        {
            if (q != null && include != null)
            {
                return await GetStudentsWithExercisesWithQ(q, include);
            }
            else if (q != null && include == null)
            {
                return await GetStudentsWithQ(q);
            }
            else if (q == null && include != null)
            {
                return await GetStudentsWithExercises(include);
            }
            else
            {
                return await GetStudents();
            }
        }
        
        /// <summary>Gets all the students from the database with an empty list of exercises</summary>
        private async Task<IActionResult> GetStudents()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
                                                    c.Name
                                            FROM Student s LEFT JOIN Cohort c on s.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Student> students = new Dictionary<int, Student>();

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        Student newStudent = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Exercises = new List<Exercise>(),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }
                        };
                        students.Add(studentId, newStudent);
                    }
                    reader.Close();
                    return Ok(students.Values);
                }
            }
        }

        // Students with q query string parameter included
        private async Task<IActionResult> GetStudentsWithQ(string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Id, c.Name 
                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        WHERE s.FirstName LIKE @q OR s.LastName LIKE @q OR s.SlackHandle LIKE @q";
                    cmd.Parameters.Add(new SqlParameter("@q", q));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Exercises = new List<Exercise>(),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }
                        };
                    }
                    reader.Close();

                    return Ok(student);
                }
            }
        }

        private async Task<IActionResult> GetStudentsWithExercisesWithQ(string q, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    if (include.ToLower() == "exercise")
                    {
                        cmd.CommandText = @"SELECT s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId, 
                                               c.Id as 'TheCohortId', c.Name as 'CohortName', 
                                            se.ExerciseId, e.Name as 'ExerciseName', e.Language
                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON se.ExerciseId = e.Id
                                        WHERE s.FirstName LIKE @q OR s.LastName LIKE @q OR s.SlackHandle LIKE @q";
                        cmd.Parameters.Add(new SqlParameter("@q", q));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Student> students = new Dictionary<int, Student>();
                        while (reader.Read())
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("TheStudentId"));
                            if (!students.ContainsKey(studentId))
                            {
                                Student newStudent = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    Exercises = new List<Exercise>(),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Cohort = new Cohort
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                        Students = new List<Student>(),
                                        Instructors = new List<Instructor>()
                                    }
                                };

                                students.Add(studentId, newStudent);
                            }

                            Student fromDictionary = students[studentId];

                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };
                                fromDictionary.Exercises.Add(exercise);
                            }

                        }

                        reader.Close();
                        return Ok(students.Values);
                    }

                    return null;
                }
            }
        }

        /// <summary>Gets all the students from the database and includes all the exercises that have been assigned to that student</summary>
        private async Task<IActionResult> GetStudentsWithExercises(string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    if (include.ToLower() == "exercise")
                    {
                        cmd.CommandText = @"SELECT s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId, 
                                               c.Id as 'TheCohortId', c.Name as 'CohortName', 
                                            se.ExerciseId, e.Name as 'ExerciseName', e.Language
                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON se.ExerciseId = e.Id";
                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Student> students = new Dictionary<int, Student>();
                        while (reader.Read())
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("TheStudentId"));
                            if (!students.ContainsKey(studentId))
                            {
                                Student newStudent = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    Exercises = new List<Exercise>(),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Cohort = new Cohort
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                        Students = new List<Student>(),
                                        Instructors = new List<Instructor>()
                                    }
                                };

                                students.Add(studentId, newStudent);
                            }

                            Student fromDictionary = students[studentId];

                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };
                                fromDictionary.Exercises.Add(exercise);
                            }

                        }

                        reader.Close();
                        return Ok(students.Values);
                    }

                    return null;
                }
            }
        }

        ///<summary>Get one student from the database based on the student's id in the url</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id, string include)
        {
            if (include != null)
            {
                return await GetStudentWithExercises(id, include);
            }
            else
            {
                return await GetStudent(id);
            }
        }


        private async Task<IActionResult> GetStudent([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Id, c.Name 
                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id WHERE s.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Exercises = new List<Exercise>(),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }
                        };
                    }
                    reader.Close();

                    return Ok(student);
                }
            }
        }

        // Get one student from the database and include all of their exercises
        private async Task<IActionResult> GetStudentWithExercises(int id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    if (include.ToLower() == "exercise")
                    {
                        cmd.CommandText = @"SELECT s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId, 
                                               c.Id as 'TheCohortId', c.Name as 'CohortName', 
                                            se.ExerciseId, e.Name as 'ExerciseName', e.Language
                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON se.ExerciseId = e.Id
                                        WHERE s.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Student> students = new Dictionary<int, Student>();
                        while (reader.Read())
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("TheStudentId"));
                            if (!students.ContainsKey(studentId))
                            {
                                Student newStudent = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    Exercises = new List<Exercise>(),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Cohort = new Cohort
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                        Students = new List<Student>(),
                                        Instructors = new List<Instructor>()
                                    }
                                };

                                students.Add(studentId, newStudent);
                            }

                            Student fromDictionary = students[studentId];

                            if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };
                                fromDictionary.Exercises.Add(exercise);
                            }

                        }

                        reader.Close();
                        return Ok(students.Values);
                    }

                    return null;
                }
            }
        }


        ///<summary>Post a new student to the database</summary>
        // POST: api/Student
        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] Student newStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName, @slackHandle, @cohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newStudent.CohortId));
                    cmd.ExecuteNonQuery();

                    return Ok(newStudent);
                }
            }
        }

        //PUT: api/student/3
        [HttpPut("{id}")]
        public void UpdateStudent([FromRoute] int id, [FromBody] Student updatedStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student
                                        SET FirstName = @firstName, LastName = @lastName, 
                                            SlackHandle = @slackHandle, CohortId = @cohortId 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", updatedStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", updatedStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", updatedStudent.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", updatedStudent.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/student/3
        [HttpDelete("{id}")]
        public void DeleteExercise([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Student WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}