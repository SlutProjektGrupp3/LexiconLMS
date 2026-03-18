using System;
using System.Collections.Generic;
using System.Text;
using Domain.Models.Entities;


namespace Service.Contracts.Courses
{
    public interface ICourseService
    {
        Task<Course> CreateCourseAsync(CreateCourseDto dto);
    }
}
