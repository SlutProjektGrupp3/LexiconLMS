using System.Collections.Generic;

namespace LMS.Shared.DTOs.Course
{
    public class CourseSummaryPagedDto
    {
        public List<CourseDetailsDto> Items { get; set; } = new();
        public int Total { get; set; }

        public int TotalActiveCourses { get; set; } = 0;
    }
}
