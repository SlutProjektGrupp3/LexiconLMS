using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.CourseDtos
{
    public class CourseCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
