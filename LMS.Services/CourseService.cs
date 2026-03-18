using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Request;
using Service.Contracts;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private IUnitOfWork uow;
        private readonly IMapper mapper;
        public CourseService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }
        
        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false)
        {
            var courses = await uow.CourseRepository.GetAllCoursesAsync(trackChanges);
            return mapper.Map<IEnumerable<CourseDto>>(courses);
        }
        public async Task<CourseDto> GetCourseAsync(Guid id, bool trackChanges = false)
        {
            var courses = await uow.CourseRepository.GetCourseAsync(id, trackChanges);
            if (courses == null)
                throw new Exception("Course not found");
            return mapper.Map<CourseDto>(courses);
        }
        public async Task<(IEnumerable<CourseDto> courseDtos, MetaData metaData)> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false)
        {
            var pagedList = await uow.CourseRepository.GetCoursesAsync(requestParams, trackChanges);
            var courseDtos = mapper.Map<IEnumerable<CourseDto>>(pagedList.Items);
            return (courseDtos, pagedList.MetaData);
        }
    }
}
