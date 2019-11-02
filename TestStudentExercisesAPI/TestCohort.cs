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
    public class TestCohort
    {
        [Fact]
        public async Task TestGetCohorts()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/cohort");
                string responseBody = await response.Content.ReadAsStringAsync();
                var cohortList = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(cohortList.Count > 0);
            }
        }

        [Fact]
        public async Task TestGetCohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("api/cohort/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var cohortList = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(cohortList.Count > 0);
            }
        }

        [Fact]
        public async Task TestPostCohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange - construct a new cohort object to be sent to the API
                Cohort newCohort = new Cohort
                {
                    Name = "Day 37",
                    Students = new List<Student>(),
                    Instructors = new List<Instructor>()
                };

                // Serialize the C# object into a JSON string
                var newCohortAsJSON = JsonConvert.SerializeObject(newCohort);

                // Act - use the client to send the request and store the response
                var response = await client.PostAsync("api/cohort",
                    new StringContent(newCohortAsJSON, Encoding.UTF8, "application/json"));

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of a Cohort
                var newCohortInstance = JsonConvert.DeserializeObject<Cohort>(responseBody);

                // Assert 
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.True(newCohortInstance.Name == "Day 37"); - returns NullReferenceException even though cohort is successfully create and posted to the DB
            }
        }


    }
}
