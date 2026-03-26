using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.CourseDtos
{
    public record CourseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}
