using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository
    {
        void CreateCourse(Course course);
        
    }
}




   

