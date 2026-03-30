using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.Modules;
using LMS.Shared.DTOs.Module;
using Domain.Models.Entities;
using Service.Contracts;

namespace LMS.Services
{
    public class ModulesService : IModuleService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IModuleRepository _moduleRepository;

        public ModulesService(IUnitOfWork uow, IMapper mapper, IModuleRepository moduleRepository)
        {
            this.uow = uow;
            this.mapper = mapper;
            _moduleRepository = moduleRepository;
        }

        public async Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto)
        {
            var module = mapper.Map<Module>(createModuleDto);
            uow.ModuleRepository.Create(module);
            
            try  
            {
                await uow.CompleteAsync();

                var createdModuleDto = mapper.Map<ModuleDto>(module);
                return CreateModuleResultDto.SuccessWith(createdModuleDto);
            }
            catch (Exception ex)
            {
                var errors = new List<ModuleError>
                {
                    new ModuleError { Code = "MODULE_ERROR:DB", Description = "An error occurred while saving the module to the database." }
                };
                return CreateModuleResultDto.Failed(errors);
            }
        }

        public async Task UpdateModuleAsync(Guid moduleId, UpdateModuleDto dto)
        {
            ValidateUpdateModule(moduleId, dto);

            var module = await _moduleRepository.GetModuleByIdAndCourseIdAsync(
                moduleId,
                dto.CourseId,
                trackChanges: true);

            if (module is null)
                throw new KeyNotFoundException("Module not found.");

            module.Name = dto.Name;
            module.Description = dto.Description;
            module.StartDate = dto.StartDate;
            module.EndDate = dto.EndDate;

            await uow.CompleteAsync();
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
    }
}
