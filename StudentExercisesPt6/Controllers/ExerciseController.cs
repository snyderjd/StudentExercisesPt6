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
    public class ExerciseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExerciseController(IConfiguration config)
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
                return await GetExercisesIncludeStudentsWithQ(q, include);
            }
            else if (q != null && include == null)
            {
                return await GetExercisesWithQ(q);
            } 
            else if (q == null && include != null)
            {
                return await GetExercisesIncludeStudents(include);
            }
            else
            {
                return await GetExercises();
            }
        }

        // Gets all exercises from the databases with no query string parameters
        private async Task<IActionResult> GetExercises()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Language FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    Exercise exercise = null;

                    while (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language")),
                            Students = new List<Student>()
                        };

                        exercises.Add(exercise);
                    }
                    reader.Close();

                    return Ok(exercises);
                }
            }
        }

        /// <summary>Gets all the exercises from the database with string parameter ?q= </summary>
        private async Task<IActionResult> GetExercisesWithQ(string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Language FROM Exercise
                                        WHERE Name LIKE @q OR Language LIKE @q";
                    cmd.Parameters.Add(new SqlParameter("@q", q));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    Exercise exercise = null;

                    while (reader.Read())
                    {
                        exercise = new Exercise(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("Language")));

                        exercises.Add(exercise);
                    }
                    reader.Close();

                    return Ok(exercises);
                    
                }
            }
        }

        private async Task<IActionResult> GetExercisesIncludeStudentsWithQ(string q, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include.ToLower() == "student")
                    {
                        cmd.CommandText = @"SELECT e.Id as 'TheExerciseId', e.Name as 'ExerciseName', e.Language,
		                                            s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId
                                            FROM Exercise e LEFT JOIN StudentExercise se ON se.ExerciseId = e.Id
				                            LEFT JOIN Student s on se.StudentId = s.Id
                                            WHERE Name LIKE @q OR Language LIKE @q";
                        cmd.Parameters.Add(new SqlParameter("@q", q));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Exercise> exercises = new Dictionary<int, Exercise>();
                        while (reader.Read())
                        {
                            int exerciseId = reader.GetInt32(reader.GetOrdinal("TheExerciseId"));
                            if (!exercises.ContainsKey(exerciseId))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = exerciseId,
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };

                                exercises.Add(exerciseId, exercise);
                            }

                            Exercise fromDictionary = exercises[exerciseId];
                            if (!reader.IsDBNull(reader.GetOrdinal("TheStudentId")))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Cohort = null
                                };

                                fromDictionary.Students.Add(student);
                            }

                        }
                        reader.Close();

                        return Ok(exercises.Values);
                    }

                    return null;

                }
            }
        }

        // Get all Exercises from the database and include list of students working on that exercise
        private async Task<IActionResult> GetExercisesIncludeStudents(string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include.ToLower() == "student")
                    {
                        cmd.CommandText = @"SELECT e.Id as 'TheExerciseId', e.Name as 'ExerciseName', e.Language,
		                                            s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId
                                            FROM Exercise e LEFT JOIN StudentExercise se ON se.ExerciseId = e.Id
				                            LEFT JOIN Student s on se.StudentId = s.Id";
                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Exercise> exercises = new Dictionary<int, Exercise>();
                        while(reader.Read())
                        {
                            int exerciseId = reader.GetInt32(reader.GetOrdinal("TheExerciseId"));
                            if (!exercises.ContainsKey(exerciseId))
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = exerciseId,
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };

                                exercises.Add(exerciseId, exercise);
                            }

                            Exercise fromDictionary = exercises[exerciseId];
                            if (!reader.IsDBNull(reader.GetOrdinal("TheStudentId")))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TheStudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Cohort = null
                                };

                                fromDictionary.Students.Add(student);
                            }

                        }
                        reader.Close();

                        return Ok(exercises.Values);
                    }

                    return null;
                    
                }
            }
        }

        /// <summary> Get one exercise from the database with the exercise id that is in the url</summary>
        [HttpGet("{id}")]
        public IActionResult GetExercise([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Language FROM Exercise WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };

                    }

                    reader.Close();
                    return Ok(exercise);
                }
            }
        }

        // POST: api/Exercise
        [HttpPost]
        public void PostExercise([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Exercise (Name, Language) VALUES (@name, @language)";
                    cmd.Parameters.Add(new SqlParameter("@name", newExercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@language", newExercise.Language));
                    cmd.ExecuteNonQuery();
                }

            }
        }

        //PUT: api/exercise/3
        [HttpPut("{id}")]
        public void UpdateExercise([FromRoute] int id, [FromBody] Exercise updatedExercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Exercise
                                        SET Name = @name, Language = @language
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@name", updatedExercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@language", updatedExercise.Language));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/exercise/3
        [HttpDelete("{id}")]
        public void DeleteExercise([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Exercise WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}