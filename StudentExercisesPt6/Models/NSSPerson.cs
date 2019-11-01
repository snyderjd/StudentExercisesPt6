using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesPt6.Models
{
    public class NSSPerson
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(24, MinimumLength = 3)]
        public string SlackHandle { get; set; }
        public Cohort Cohort { get; set; }
    }
}
