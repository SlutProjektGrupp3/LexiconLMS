using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using LMS.Shared.DTOs;


namespace Service.Contracts.Courses
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    }
}
