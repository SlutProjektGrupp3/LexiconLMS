using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Course
{
    public class AddStudentToCourseDto
    {
        public Guid CourseId { get; set; }
        public string StudentId { get; set; } = string.Empty;
    }
}
