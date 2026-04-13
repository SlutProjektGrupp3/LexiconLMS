using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.Module;
using Service.Contracts;

namespace LMS.Services;

public class ModulesService : IModuleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleRepository;

    public ModulesService(IUnitOfWork uow, IMapper mapper, IModuleRepository moduleRepository)
    {
        _uow = uow;
        _mapper = mapper;
        _moduleRepository = moduleRepository;
    }

        public async Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto)
        {
            var course = await _uow.CourseRepository.GetCourseByIdAsync(createModuleDto.CourseId, trackChanges: false);

            if (course is null)
            {
                return CreateModuleResultDto.Failed(new List<ModuleError>
                {
                    new ModuleError
                    {
                        Code = "CourseNotFound",
                        Description = $"Course with id {createModuleDto.CourseId} was not found."
                    }
                });
            }

            var module = _mapper.Map<Module>(createModuleDto);
            module.CourseId = course.Id;
            _uow.ModuleRepository.Create(module);

        try
        {
            await _uow.CompleteAsync();

                var createdModuleDto = _mapper.Map<ModuleDto>(module);
                return CreateModuleResultDto.SuccessWith(createdModuleDto);
            }
            catch (Exception ex)
            {
                return CreateModuleResultDto.Failed(new List<ModuleError>
                {
                    new ModuleError
                    {
                        Code = "DatabaseError",
                        Description = "An error occurred while saving the module."
                    }
                });
            }
        }

        public async Task<DeleteModuleResultDto> DeleteModuleAsync(Guid moduleId)
        {
            if (moduleId == Guid.Empty)
            {
                return DeleteModuleResultDto.Failed(new ModuleError
                {
                    Code = "DatabaseError",
                    Description = "Module ID is required and cannot be empty.",
                    StatusCode = ErrorStatusCode.BadRequest
                });
            }

        var module = await _uow.ModuleRepository.GetModuleByIdAsync(moduleId, trackChanges: false);
        if (module != null)
        {
            _uow.ModuleRepository.Delete(module);

            try
            {
                await _uow.CompleteAsync();
                return DeleteModuleResultDto.Success;
            }
            catch (Exception ex)
            {
                return DeleteModuleResultDto.Failed(new ModuleError
                {
                    Code = "MODULE_ERROR:DB",
                    Description = "An error occurred while deleting the module to the database.",
                    StatusCode = ErrorStatusCode.Database
                });
            }
        }

        return DeleteModuleResultDto.Failed(new ModuleError
        {
            Code = "MODULE_ERROR:DELETE",
            Description = "Can't delete module: not found",
            StatusCode = ErrorStatusCode.NotFound
        });
     }   
        
    public async Task UpdateModuleAsync(Guid moduleId, UpdateModuleDto dto)
    {
        ValidateUpdateModule(moduleId, dto);

        var module = await _moduleRepository.GetModuleByIdAndCourseIdAsync(
            moduleId,
            dto.CourseId,
            trackChanges: true);

        if (module is null)
            throw new NotFoundException($"Module with id {moduleId} was not found.");

        _mapper.Map(dto, module);

        await _uow.CompleteAsync();
    }

    private static void ValidateUpdateModule(Guid moduleId, UpdateModuleDto dto)
    {
        if (dto.Id != moduleId)
            throw new ArgumentException("ModuleId in route and body do not match.");

        if (dto.CourseId == Guid.Empty)
            throw new ArgumentException("CourseId is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Module name is required.");

        if (dto.StartDate == default)
            throw new ArgumentException("Start date is required.");

        if (dto.EndDate == default)
            throw new ArgumentException("End date is required.");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("End date must be after start date.");
    }
    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        var module = await _uow.ModuleRepository.GetByIdAsync(id, trackChanges: false);

        if (module is null)
            return null;

        return _mapper.Map<ModuleDto>(module);
    }

}