using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesPt6.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Language { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();

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
