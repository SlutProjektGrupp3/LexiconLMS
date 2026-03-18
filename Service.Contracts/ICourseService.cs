using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseDto> courseDtos, MetaData metaData)> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync (bool trackChanges = false);
        Task<CourseDto> GetCourseAsync (Guid courseId, bool trackChanges = false);

    }
}
