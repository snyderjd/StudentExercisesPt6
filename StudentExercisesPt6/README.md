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
