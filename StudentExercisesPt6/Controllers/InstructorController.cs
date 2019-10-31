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
    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
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

        //GET: api/instructor
        /// <summary>Gets all the instructors from the database</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllInstructors(string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Specialty, i.CohortId, c.Id,                         c.Name 
                                        FROM Instructor i LEFT JOIN Cohort c ON i.CohortId = c.Id
                                        WHERE i.FirstName LIKE @q OR i.LastName LIKE @q OR i.SlackHandle LIKE @q";

                    cmd.Parameters.Add(new SqlParameter("@q", q));
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Instructor> instructors = new List<Instructor>();
                    Instructor instructor = null;

                    while (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }

                        };

                        instructors.Add(instructor);
                    }
                    reader.Close();
                    return Ok(instructors);              

                }
            }
        }

        ///<summary>Get one instructor from the database based on the instructor's id in the url</summary>
        // GET(id): /api/instructor/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstructor([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Specialty, i.CohortId, c.Id, c.Name 
                                        FROM Instructor i LEFT JOIN Cohort c ON i.CohortId = c.Id WHERE i.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
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

                    return Ok(instructor);
                }
            }
        }

        ///<summary>Post a new instructor to the database</summary>
        // POST: api/Instructor
        [HttpPost]
        public async void PostInstructor([FromBody] Instructor newInstructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId)
                                        VALUES (@firstName, @lastName, @slackHandle, @specialty, @cohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newInstructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@specialty", newInstructor.Specialty));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newInstructor.CohortId));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Edit an instructor in the database using the id in the url</summary>
        // PUT: api/instructor/3
        [HttpPut("{id}")]
        public async void UpdateInstructor([FromRoute] int id, [FromBody] Instructor updatedInstructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Instructor
                                        SET FirstName = @firstName, LastName = @lastName, SlackHandle = @slackHandle,
                                            Specialty = @specialty, CohortId = @cohortId
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", updatedInstructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", updatedInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", updatedInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@specialty", updatedInstructor.Specialty));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", updatedInstructor.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Delete an instructor from the database based on the id in the url</summary>
        // DELETE api/instructor/3
        [HttpDelete("{id}")]
        public async void DeleteInstructor([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Instructor WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}