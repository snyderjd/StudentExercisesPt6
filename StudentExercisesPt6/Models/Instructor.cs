﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesPt6.Models
{
    public class Instructor : NSSPerson
    {
        // Properties
        public string Specialty { get; set; }
        public int Id { get; set; }

        // Constructor
        public Instructor(int id, string firstName, string lastName, string slackHandle, Cohort cohort, string specialty)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            SlackHandle = slackHandle;
            Specialty = specialty;
            Cohort = cohort;
        }

        // Method to assign an exercise to a student
        public void AssignExercise(Exercise exercise, Student student)
        {
            student.Exercises.Add(exercise);
        }
    }
}
