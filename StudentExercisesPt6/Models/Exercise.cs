﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesPt6.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public Exercise(int id, string name, string language)
        {
            Id = id;
            Name = name;
            Language = language;
        }

        public Exercise()
        {

        }
    }
}
