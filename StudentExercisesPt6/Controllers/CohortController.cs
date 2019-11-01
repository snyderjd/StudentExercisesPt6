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

        [HttpGet]
        public async Task<IActionResult> Get(string q)
        {
            if (q != null)
            {
                // q parameter is used/passed in
                return await GetCohortsWithQ(q);
            }
            else
            {
                // q parameter is not used
                return await GetCohorts();
            }
        }

        // Get cohorts with use of q query string parameter
        private async Task<IActionResult> GetCohortsWithQ(string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as 'TheCohortId', c.Name as 'CohortName',
		                                        s.Id as 'TheStudentId', s.FirstName as 'StudentFirstName', s.LastName as                            'StudentLastName', s.SlackHandle as 'StudentSlackHandle',
		                                        i.Id as 'TheInstructorId', i.FirstName as 'InstructorFirstName', i.LastName as                      'InstructorLastName', i.SlackHandle as 'InstructorSlackHandle', i.Specialty
                                                FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id LEFT JOIN Instructor i ON                    i.CohortId = c.Id
                                                WHERE c.Name LIKE @q";
                    cmd.Parameters.Add(new SqlParameter("@q", q));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            };

                            cohorts.Add(cohortId, cohort);
                        }

                        Cohort fromDictionary = cohorts[cohortId];

                        if (!reader.IsDBNull(reader.GetOrdinal("TheStudentId")))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("TheStudentId"));

                            if (! cohorts[cohortId].Students.Any(s => s.Id == studentId))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                    Cohort = null,
                                    Exercises = new List<Exercise>()
                                };
                                fromDictionary.Students.Add(student);
                            }
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TheInstructorId")))
                        {
                            int instructorId = reader.GetInt32(reader.GetOrdinal("TheInstructorId"));

                            if (!cohorts[cohortId].Instructors.Any(i => i.Id == instructorId))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheInstructorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                    Cohort = null
                                };
                                fromDictionary.Instructors.Add(instructor);
                            }
                        }
                    }
                    reader.Close();

                    return Ok(cohorts.Values);
                }
            }
        }

        ///<summary>Gets all the cohorts from the database</summary>
        // GET: api/cohort
        private async Task<IActionResult> GetCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as 'TheCohortId', c.Name as 'CohortName',
		                                        s.Id as 'TheStudentId', s.FirstName as 'StudentFirstName', s.LastName as                            'StudentLastName', s.SlackHandle as 'StudentSlackHandle',
		                                        i.Id as 'TheInstructorId', i.FirstName as 'InstructorFirstName', i.LastName as                      'InstructorLastName', i.SlackHandle as 'InstructorSlackHandle', i.Specialty
                                                FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id LEFT JOIN Instructor i ON                    i.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    while(reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            };

                            cohorts.Add(cohortId, cohort);
                        }

                        Cohort fromDictionary = cohorts[cohortId];

                        if (! reader.IsDBNull(reader.GetOrdinal("TheStudentId")))
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                Cohort = null,
                                Exercises = new List<Exercise>()
                            };
                            fromDictionary.Students.Add(student);
                        }

                        if (! reader.IsDBNull(reader.GetOrdinal("TheInstructorId")))
                        {
                            Instructor instructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheInstructorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                Cohort = null
                            };
                            fromDictionary.Instructors.Add(instructor);
                        }
                    }
                    reader.Close();

                    return Ok(cohorts.Values);
                }
            }
        }

        ///<summary>Get one cohort from the database based on the cohort's id in the url</summary>
        // GET(id): api/cohort/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCohort([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as 'TheCohortId', c.Name as 'CohortName',
		                                        s.Id as 'TheStudentId', s.FirstName as 'StudentFirstName', s.LastName as                            'StudentLastName', s.SlackHandle as 'StudentSlackHandle',
		                                        i.Id as 'TheInstructorId', i.FirstName as 'InstructorFirstName', i.LastName as                      'InstructorLastName', i.SlackHandle as 'InstructorSlackHandle', i.Specialty
                                                FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id LEFT JOIN Instructor i ON                    i.CohortId = c.Id
                                                WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            };

                            cohorts.Add(cohortId, cohort);
                        }

                        Cohort fromDictionary = cohorts[cohortId];

                        if (!reader.IsDBNull(reader.GetOrdinal("TheStudentId")))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("TheStudentId"));

                            if (!cohorts[cohortId].Students.Any(s => s.Id == studentId))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                    Cohort = null,
                                    Exercises = new List<Exercise>()
                                };
                                fromDictionary.Students.Add(student);
                            }
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TheInstructorId")))
                        {
                            int instructorId = reader.GetInt32(reader.GetOrdinal("TheInstructorId"));

                            if(!cohorts[cohortId].Instructors.Any(i => i.Id == instructorId))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheInstructorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("TheCohortId")),
                                    Cohort = null
                                };
                                fromDictionary.Instructors.Add(instructor);
                            }
                        }
                    }
                    reader.Close();

                    return Ok(cohorts.Values);
                }
            }
        }

        ///<summary>Post a new cohort to the database</summary>
        // POST: api/cohort
        [HttpPost]
        public async void PostCohort([FromBody] Cohort newCohort)
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
        public async void UpdateCohort([FromRoute] int id, [FromBody] Cohort updatedCohort)
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
        public async void DeleteCohort([FromRoute] int id)
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