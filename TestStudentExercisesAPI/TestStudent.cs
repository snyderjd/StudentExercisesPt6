using Newtonsoft.Json;
using StudentExercisesPt6.Models;
using System;
using System.Collections.Generic;
using System.Net;
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
    }
}
