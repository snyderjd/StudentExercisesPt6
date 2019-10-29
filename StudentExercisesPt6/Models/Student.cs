using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesPt6.Models
{
    public class Student
    {
        public List<Exercise> Exercises { get; set; }
        public int Id { get; set; }

        public Student(int id, string firstName, string lastName, string slackHandle, Cohort cohort)
        {
            Exercises = new List<Exercise>();
            FirstName = firstName;
            LastName = lastName;
            SlackHandle = slackHandle;
            Cohort = cohort;
            Id = id;
        }
    }
}
