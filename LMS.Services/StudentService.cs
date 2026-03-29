using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Module;
using Service.Contracts;

namespace LMS.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StudentMyCourseDto?> GetMyCourseAsync(Guid userId)
        {
            var student = await _unitOfWork.StudentRepository.GetStudentWithCourseAsync(userId, trackChanges: false);

            if (student is null || student.Course is null)
                return null;

            return _mapper.Map<StudentMyCourseDto?>(student.Course);            
        }
    }
}