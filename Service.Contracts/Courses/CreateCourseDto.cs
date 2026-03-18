using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Service.Contracts.Courses
{
    public class CreateCourseDto
    {

        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
      
    }
}
