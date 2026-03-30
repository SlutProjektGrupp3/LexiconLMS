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

        public ModulesService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
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

        public async Task<DeleteModuleResultDto> DeleteModuleAsync(Guid moduleId)
        {
            if (moduleId == Guid.Empty)
            {
                return DeleteModuleResultDto.Failed(new ModuleError
                {
                    Code = "MODULE_ERROR:VALIDATION",
                    Description = "Module ID is required and cannot be empty.",
                    StatusCode = ErrorStatusCode.BadRequest
                });
            }

            var module = await uow.ModuleRepository.GetModuleByIdAsync(moduleId, trackChanges: false);
            if (module != null)
            {
                uow.ModuleRepository.Delete(module);

                try
                {
                    await uow.CompleteAsync();
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
    }
}