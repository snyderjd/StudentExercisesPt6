using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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

        ///<summary>Gets all the cohorts from the database</summary>
        // GET: api/cohort
        [HttpGet]
        public List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Create an object for each cohort and instantiate lists for instructors and students
                    cmd.CommandText = @"SELECT Id, Name FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    Cohort cohort = null;
                    
                    while (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Instructors = new List<Instructor>(),
                            Students = new List<Student>()
                        };

                        cohorts.Add(cohort);
                    }
                    reader.Close();

                    // Get all the students from the database
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, CohortId
                                        FROM Student";
                   
                    Student student = null;
                    SqlDataReader reader2 = cmd.ExecuteReader();

                    // Iterate over the data that was received from the students SQL query
                    while (reader2.Read())
                    {
                        // Instantiate a new student
                        student = new Student
                        {
                            Id = reader2.GetInt32(reader2.GetOrdinal("Id")),
                            FirstName = reader2.GetString(reader2.GetOrdinal("FirstName")),
                            LastName = reader2.GetString(reader2.GetOrdinal("LastName")),
                            SlackHandle = reader2.GetString(reader2.GetOrdinal("SlackHandle")),
                            CohortId = reader2.GetInt32(reader2.GetOrdinal("CohortId")),
                            Cohort = null,
                            Exercises = new List<Exercise>()
                        };

                        // Find the cohort that has an Id that matches the student's cohortId
                        Cohort matchedCohort = cohorts.First(cohort => cohort.Id == student.CohortId);

                        // Add the student to the correct cohort
                        matchedCohort.Students.Add(student);
                    }
                    reader2.Close();

                    // Get all the instructors from the database
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, Specialty, CohortId
                                        FROM Instructor";
                    SqlDataReader reader3 = cmd.ExecuteReader();
                    Instructor instructor = null;

                    // Iterate over the data that was received from the instructors SQL query
                    while (reader3.Read())
                    {
                        // Instantiate a new instructor
                        instructor = new Instructor
                        {
                            Id = reader3.GetInt32(reader3.GetOrdinal("Id")),
                            FirstName = reader3.GetString(reader3.GetOrdinal("FirstName")),
                            LastName = reader3.GetString(reader3.GetOrdinal("LastName")),
                            SlackHandle = reader3.GetString(reader3.GetOrdinal("SlackHandle")),
                            Specialty = reader3.GetString(reader3.GetOrdinal("Specialty")),
                            CohortId = reader3.GetInt32(reader3.GetOrdinal("CohortId"))
                        };

                        Cohort matchedCohort = cohorts.First(cohort => cohort.Id == instructor.CohortId);
                        matchedCohort.Instructors.Add(instructor);
                    }
                    reader3.Close();

                    return cohorts;
                }
            }
        }

        ///<summary>Get one cohort from the database based on the cohort's id in the url</summary>
        // GET(id): api/cohort/3
        [HttpGet("{id}")]
        public IActionResult GetCohort([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;
                    
                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Instructors = new List<Instructor>(),
                            Students = new List<Student>()
                        };
                    }
                    reader.Close();
                    return Ok(cohort);
                }
            }
        }

        ///<summary>Post a new cohort to the database</summary>
        // POST: api/cohort
        [HttpPost]
        public void PostCohort([FromBody] Cohort newCohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Cohort (Name) VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", newCohort.Name));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Edit a cohort in the database using the id in the url</summary>
        // PUT: api/cohort/3
        [HttpPut("{id}")]
        public void UpdateCohort([FromRoute] int id, [FromBody] Cohort updatedCohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cohort
                                        SET Name = @name
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@name", updatedCohort.Name));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Delete a cohort from the database based on the id in the url</summary>
        // DELETE api/cohort/3
        [HttpDelete("{id}")]
        public void DeleteCohort([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}