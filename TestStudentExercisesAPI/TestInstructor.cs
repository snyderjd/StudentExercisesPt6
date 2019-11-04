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
    public class TestInstructor
    {
        [Fact]
        public async Task TestGetInstructors()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/instructor");
                string responseBody = await response.Content.ReadAsStringAsync();
                var instructorList = JsonConvert.DeserializeObject<List<Instructor>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(instructorList.Count > 0);
            }
        }

        [Fact]
        public async Task TestGetInstructorsWithQ()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/instructor?q=Bryan");
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    List<Instructor> instructorList = JsonConvert.DeserializeObject<List<Instructor>>(responseBody);
                }
                catch
                {
                    var instructor = JsonConvert.DeserializeObject<Instructor>(responseBody);
                }

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetInstructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("api/instructor/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var instructor = JsonConvert.DeserializeObject<Instructor>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.True(instructorList.Count > 0);
            }
        }

        [Fact]
        public async Task TestPostInstructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange - construct a new instructor object to be sent to the API
                Instructor newInstructor = new Instructor
                {
                    FirstName = "Mark",
                    LastName = "Zuckerberg",
                    SlackHandle = "MZuckerberg",
                    Specialty = "Wearing sweatshirts",
                    CohortId = 3
                };

                // Serialize the C# object into a JSON string
                var newInstructorAsJSON = JsonConvert.SerializeObject(newInstructor);

                // Act - use the client to send the request and store the response
                var response = await client.PostAsync("api/instructor",
                    new StringContent(newInstructorAsJSON, Encoding.UTF8, "application/json"));

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of an Instructor
                var newInstructorInstance = JsonConvert.DeserializeObject<Instructor>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestPutInstructor()
        {
            // new SlackHandle to change to and test
            string slackHandle = "@MarkZ";
            using (var client = new APIClientProvider().Client)
            {
                // PUT section
                Instructor updatedInstructor = new Instructor
                {
                    FirstName = "Mark",
                    LastName = "Zuckerberg",
                    CohortId = 3,
                    Specialty = "Wearing hoodies",
                    SlackHandle = slackHandle
                };

                var modifiedInstructorAsJSON = JsonConvert.SerializeObject(updatedInstructor);

                var response = await client.PutAsync("api/instructor/11",
                    new StringContent(modifiedInstructorAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // GET section - verify that the PUT operation was successful
                var getInstructor = await client.GetAsync("/api/instructor/11");
                getInstructor.EnsureSuccessStatusCode();

                string getInstructorBody = await getInstructor.Content.ReadAsStringAsync();
                Instructor newInstructor = JsonConvert.DeserializeObject<Instructor>(getInstructorBody);

                Assert.Equal(HttpStatusCode.OK, getInstructor.StatusCode);
                Assert.Equal(slackHandle, newInstructor.SlackHandle);
            }
        }

        [Fact]
        public async Task TestDeleteInstructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                // get num of instructors currently in the DB
                var response = await client.GetAsync("/api/instructor");
                string responseBody = await response.Content.ReadAsStringAsync();
                var instructorList = JsonConvert.DeserializeObject<List<Instructor>>(responseBody);

                int deleteId = 11; //id of instructor to delete

                // Act
                var response2 = await client.DeleteAsync($"/api/instructor/{deleteId}");
                string responseBody2 = await response.Content.ReadAsStringAsync();
                var instructor = JsonConvert.DeserializeObject<List<Instructor>>(responseBody)[0];

                var response3 = await client.GetAsync("/api/instructor");
                string responseBody3 = await response.Content.ReadAsStringAsync();
                var instructorList2 = JsonConvert.DeserializeObject<List<Instructor>>(responseBody3);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.True(instructorList2.Count < instructorList.Count);
            }
        }

        //[Fact]
        //public async Task TestDeleteCohort()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        int deleteId = 7;

        //        // Act
        //        var response = await client.DeleteAsync($"/api/cohort/{deleteId}");
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        var cohort = JsonConvert.DeserializeObject<Cohort>(responseBody);

        //        // Assert
        //        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    }
        //}

    }
}
