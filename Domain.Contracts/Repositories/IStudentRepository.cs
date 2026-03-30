using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface IStudentRepository
    {
        Task<ApplicationUser?> GetStudentWithCourseAsync(Guid userId, bool trackChanges = false);
    }
}