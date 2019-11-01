## Starting the Student Exercises API

Your instruction team will get you started on converting your student exercises command line app into an API that will respond to HTTP requests.

### Create the Project

Follow the steps from the beginning of the lesson to start a new Web API project from Visual Studio or the command line - whichever is your preference. Name the project `StudentExercisesAPI`.

### Connecting to Database

1. Update `appsettings.json`
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Warning"
       }
     },
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StudentExercises;Trusted_Connection=True;"
     }
   }
   ```

### Exercise Model

Copy the `Models/Exercise.cs` file from the CLI application into the `Models` directory of your new API project.

### Controller Methods

1. Create `ExercisesController`
1. Code for getting a list of exercises
1. Code for getting a single exercise
1. Code for creating an exercise
1. Code for editing an exercise
1. Code for deleting an exercise

# Student Exercises Controllers

Your task is to continue to build out the Student Exercises API.

1. Copy remaining models from your CLI application to `Models` directory of your API project.
1. Create Student controller. Student JSON representation should include cohort.
    ```json
    {
        "id": 1,
        "firstName": "Jane",
        "lastName": "Doe",
        "slackHandle": "@jane",
        "cohortId": 1,
        "cohort": {
            "id": 1,
            "name": "Cohort 28",
            "students": [],
            "instructors": []
        },
        "exercises": []
    }
    ```
1. Create Instructor controller. Instructor JSON representation should include cohort.
    ```json
    {
        "id": 1,
        "firstName": "Jisie",
        "lastName": "David",
        "slackHandle": "@jisie",
        "cohortId": 2,
        "cohort": {
            "id": 2,
            "name": "Cohort 29",
            "students": [],
            "instructors": []
        }
    },
    ```
1. Create Cohort controller. Cohort JSON representation should include an array of students, and an array of instructors.
    ```json
    {
        "name": "Cohort 29",
        "Students": [
            {
                "id": 4,
                "firstName": "Daniel",
                "lastName": "Brewer",
                "slackHandle": "@dan",
                "cohortId": 2,
                "cohort": null,
                "exercises": []
            },
            {
                "id": 5,
                "firstName": "JD",
                "lastName": "Wheeler",
                "slackHandle": "@jd",
                "cohortId": 2,
                "cohort": null,
                "exercises": []
            }
        ],
        "Instructor": [
            {
                "id": 1,
                "firstName": "Jisie",
                "lastName": "David",
                "slackHandle": "@jisie",
                "cohortId": 2,
                "cohort": null
            },
            {
                "id": 2,
                "firstName": "Andy",
                "lastName": "Collins",
                "slackHandle": "@andy",
                "cohortId": 2,
                "cohort": null
            }
        ]
    }
    ```

# Student Exercises Part 7 - Using Query String Parameters

## Practice

1. Student JSON response should have all exercises that are assigned to them if the `include=exercise` query string parameter is there.
1. Exercise JSON response should have all currently assigned students if the `include=students` query string parameter is there.
1. Provide support for each resource (Instructor, Student, Cohort, Exercise) and the `q` query string parameter. If it is provided, your SQL should search relevant property for a match, search all properties of the resource for a match.
    1. `FirstName`, `LastName`, and `SlackHandle` for instructors and students.
    1. `Name` and `Language` for exercises.
    1. `Name` for cohorts.


> **Hint:** Use [LIKE](https://www.techonthenet.com/sql_server/like.php) in the SQL query for pattern matching.

# Part 8 - Validating Student Exercise Data

## Practice

1. `Name` and `Language` properties on an exercises should be required.
1. Instructor `FirstName`, `LastName`, and `SlackHandle` should be required.
1. Cohort `Name` should be required.
1. Cohort `Name` should be a minimum of of 5 characters and and no more than 11.
1. Student, and Instructor `SlackHandle` value should be a minimum of 3 characters and no more than 12.

## Challenge: Regular Expressions

Regular expressions are a useful machanism in all software languages that allow you to do advanced pattern matching. Visit [https://regexr.com/](https://regexr.com/) and [https://www.regular-expressions.info/](https://www.regular-expressions.info/) and find a way to write a regular expression validation rule for the following requirement.

1. Cohort `Name` property must be two words. The first word should be `Day` or `Evening`. The second word must be a 1 or 2 digit number. The `d` and `e` at the beginning of the first word can be lowercase or uppercase.
1. If the validation fails, the client should get the error message _"Cohort name should be in the format of [Day|Evening] [number]"_

# Part 9 - Testing Student Exercise API

## Practice

You need to create integration tests for the following features of your Student Exercises API.

1. Basic CRUD for Students
1. Basic CRUD for Cohorts
1. Basic CRUD for Instructors
1. Basic CRUD for Exercises

## Challenge: Managing Assignments

The relationship between students and exercises is a many-to-many type, which requires an intesection table to store those relationships. Usually, but not always, those relationships aren't exposed by your application for reading or manipulation by a client. This means that there is no **Controller** for intersection tables. The items in intersection tables are managed by other controllers.

In this API, however, you need a way to assign an exercise to a student - which is done by an instructor. Therefore, you need to provide an endpoint that a client can use to make the assignment. You can make an **`AssignmentController`** that provides support for the following actions.

1. **Assign exercise:** Support POST operation that will create a new entry in `StudentExercise` table.
1. **Unassign exercise:** Support DELETE operation that will remove a relationship by its `Id`.

Once that controller is created, add a new test file to your testing project named `TestAssginments.cs`. Then build a **`TestAssignments`** class that verifies that the operations are working by making the following assertions pass.

```cs
public class TestAssignments
{
    [Fact]
    public async Task Test_Assign_Exercise_To_Student()
    {

        using (var client = new APIClientProvider().Client)
        {


            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(1, newAssignment.StudentId);
            Assert.Equal(3, newAssignment.ExerciseId);
            Assert.Equal(2, newAssignment.InstructorId);
        }
    }

    [Fact]
    public async Task Test_Unassign_Exercise()
    {

        using (var client = new APIClientProvider().Client)
        {

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
```