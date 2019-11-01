using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesPt6.Models
{
    public class Cohort
    {
        [Required]
        [StringLength(11, MinimumLength = 5)]
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Student> Students { get; set; }
        public List<Instructor> Instructors { get; set; }

        public Cohort(int id, string name)
        {
            Id = id;
            Name = name;
            Students = new List<Student>();
            Instructors = new List<Instructor>();
        }

        public Cohort()
        {

        }
    }
}
