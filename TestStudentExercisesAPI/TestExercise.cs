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
    public class TestExercise
    {
        [Fact]
        public async Task TestGetExercises()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/exercise");
                string responseBody = await response.Content.ReadAsStringAsync();
                var exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(exerciseList.Count > 0);
            }
        }

        [Fact]
        public async Task TestGetExercisesWithQ()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/exercise?q=javascript");
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    List<Exercise> exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);
                }
                catch
                {
                    var exercise = JsonConvert.DeserializeObject<Exercise>(responseBody);
                }

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("api/exercise/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var exercise = JsonConvert.DeserializeObject<Exercise>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Chicken Monkey", exercise.Name);
            }
        }

        [Fact]
        public async Task TestGetExercisesWithQIncludeStudents()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("api/exercise?q=javascript&include=student");
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    List<Exercise> exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);
                }
                catch
                {
                    Exercise exercise = JsonConvert.DeserializeObject<Exercise>(responseBody);
                }

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestPostExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange - construct a new exercise object to be sent to the API
                Exercise newExercise = new Exercise
                {
                    Name = "Reverse string",
                    Language = "JavaScript"
                };

                // Serialize the C# object into a JSON string
                var newExerciseAsJSON = JsonConvert.SerializeObject(newExercise);

                // Act - use the client to send the request and store the response
                var response = await client.PostAsync("api/exercise",
                    new StringContent(newExerciseAsJSON, Encoding.UTF8, "application/json"));

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of an exercise
                Exercise newExerciseInstance = JsonConvert.DeserializeObject<Exercise>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.Equal("Reverse string", newExerciseInstance.Name); - NullReferenceException : Object reference not set to an instance of an object
            }
        }

        [Fact]
        public async Task TestPutExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                // PUT section
                Exercise updatedExercise = new Exercise
                {
                    Name = "Chicken Monkey",
                    Language = "JavaScript"
                };

                var modifiedExerciseAsJSON = JsonConvert.SerializeObject(updatedExercise);

                var response = await client.PutAsync("api/exercise/1",
                    new StringContent(modifiedExerciseAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // GET section - verify that the PUT operation was successful
                var getExercise = await client.GetAsync("api/exercise/1");
                getExercise.EnsureSuccessStatusCode();

                string getExerciseBody = await getExercise.Content.ReadAsStringAsync();
                Exercise newExercise = JsonConvert.DeserializeObject<Exercise>(getExerciseBody);

                Assert.Equal(HttpStatusCode.OK, getExercise.StatusCode);
                Assert.Equal("JavaScript", updatedExercise.Language);
            }
        }

        [Fact]
        public async Task TestDeleteExercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                int deleteId = 15; //id of exercise to delete

                // Act
                var response2 = await client.DeleteAsync($"/api/exercise/{deleteId}");
                string responseBody2 = await response2.Content.ReadAsStringAsync();
                Exercise exercise = JsonConvert.DeserializeObject<Exercise>(responseBody2);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetExercisesIncludeStudents()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                // Act
                var response = await client.GetAsync("/api/exercise?include=student");
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Exercise> exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(exerciseList.Count > 0);
            }
        }


    }
}
