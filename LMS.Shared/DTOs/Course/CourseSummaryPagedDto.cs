using System.Collections.Generic;

namespace LMS.Shared.DTOs.Course
{
    public class CourseSummaryPagedDto
    {
        public List<CourseSummaryDto> Items { get; set; } = new();
        public int Total { get; set; }
    }
}
