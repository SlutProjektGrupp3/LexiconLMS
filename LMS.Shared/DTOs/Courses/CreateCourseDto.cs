using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace LMS.Shared.DTOs
{
    public class CreateCourseDto
    {

        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
      
    }
}
