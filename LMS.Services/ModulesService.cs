using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.Modules;
using Domain.Models.Entities;
using Service.Contracts;

namespace LMS.Services
{
    public class ModulesService : IModulesService
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
                return CreateModuleResultDto.Success;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                var errors = new List<ModuleError>
                {
                    new ModuleError { Code = "MODULE_ERROR:DB", Description = "An error occurred while saving the module to the database." }
                };
                return CreateModuleResultDto.Failed(errors);
            }

            throw new NotImplementedException();
        }
    }
}