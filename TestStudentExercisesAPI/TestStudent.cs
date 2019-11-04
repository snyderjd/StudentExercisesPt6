using Newtonsoft.Json;
using StudentExercisesPt6.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestStudentExercisesPt6;
using Xunit;

namespace TestStudentExercisesAPI
{
    public class TestStudent
    {
        [Fact]
        public async Task TestGetStudents()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange

                // Act
                var response = await client.GetAsync("/api/student");
                string responseBody = await response.Content.ReadAsStringAsync();
                var studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(studentList.Count > 0);
            }
        }

        [Fact]
        public async Task TestGetStudentsWithQ()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/student?q=joseph");
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var student = JsonConvert.DeserializeObject<Student>(responseBody);
                }
                catch
                {
                    List<Student> studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);
                }

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetStudentsWithQIncludeExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/student?q=joseph&include=exercise");
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var student = JsonConvert.DeserializeObject<Student>(responseBody);
                }
                catch
                {
                    List<Student> studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);
                }

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetStudentsWithExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange

                // Act
                var response = await client.GetAsync("/api/student?include=exercise");
                string responseBody = await response.Content.ReadAsStringAsync();
                var studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(studentList.Count > 0);
            }
        }

        [Fact]
        public async Task TestGetStudent()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange

                // Act
                var response = await client.GetAsync("/api/student/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var student = JsonConvert.DeserializeObject<Student>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(student != null);
                Assert.True(student.FirstName.Length > 0);
                Assert.True(student.LastName.Length > 0);
            }
        }

        [Fact]
        public async Task TestPostStudent()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // construct a new student object to be sent to the API
                Student newStudent = new Student
                {
                    FirstName = "Aaron",
                    LastName = "Rodgers",
                    SlackHandle = "AaronR",
                    CohortId = 1,
                    Exercises = new List<Exercise>()
                };

                // Serialize the C# object into a JSON string
                var newStudentAsJSON = JsonConvert.SerializeObject(newStudent);

                // Act
                // Use the client to send the request and store the response
                var response = await client.PostAsync("api/student",
                    new StringContent(newStudentAsJSON, Encoding.UTF8, "application/json"));

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of a Student
                var newStudentInstance = JsonConvert.DeserializeObject<Student>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Aaron", newStudentInstance.FirstName);
                Assert.Equal("Rodgers", newStudentInstance.LastName);
                Assert.Equal("AaronR", newStudentInstance.SlackHandle);
                Assert.Equal(1, newStudentInstance.CohortId);

            }
        }

        [Fact]
        public async Task TestPutStudent()
        {
            // new FirstName to change to and test
            string FirstName = "Joseph";

            using (var client = new APIClientProvider().Client)
            {
                // PUT section
                Student updatedStudent = new Student
                {
                    FirstName = FirstName,
                    LastName = "Snyder",
                    SlackHandle = "JoeS",
                    CohortId = 2
                };

                var modifiedStudentAsJSON = JsonConvert.SerializeObject(updatedStudent);

                var response = await client.PutAsync(
                    "api/student/7",
                    new StringContent(modifiedStudentAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // GET section - verify that the PUT operation was successful
                var getStudent = await client.GetAsync("/api/student/7");
                getStudent.EnsureSuccessStatusCode();

                string getStudentBody = await getStudent.Content.ReadAsStringAsync();
                Student newStudent = JsonConvert.DeserializeObject<Student>(getStudentBody);

                Assert.Equal(HttpStatusCode.OK, getStudent.StatusCode);
                Assert.Equal(FirstName, newStudent.FirstName);

            }
        }

        [Fact]
        public async Task TestDeleteStudent()
        {
            using (var client = new APIClientProvider().Client)
            {
                int deleteId = 14;
                // Arrange

                // Act
                var response = await client.DeleteAsync($"/api/student/{deleteId}");
                string responseBody = await response.Content.ReadAsStringAsync();
                var student = JsonConvert.DeserializeObject<Student>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        
    }
}
