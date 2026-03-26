using LMS.Shared.DTOs.Module;

namespace LMS.Shared.DTOs.Modules
{
    public class CreateModuleResultDto
    {
        private readonly List<ModuleError> _errors = new List<ModuleError>();

        public bool Succeeded { get; set; }

        public IEnumerable<ModuleError> Errors => _errors;

        // Return a fresh success instance to avoid mutating a shared singleton
        public static CreateModuleResultDto Success => new CreateModuleResultDto { Succeeded = true };

        public ModuleDto? CreatedModule { get; set; } = null;

        // Factory method to create a success result that includes the created module
        public static CreateModuleResultDto SuccessWith(ModuleDto module) =>
            new CreateModuleResultDto { Succeeded = true, CreatedModule = module };

        public static CreateModuleResultDto Failed(List<ModuleError>? errors)
        {
            var result = new CreateModuleResultDto { Succeeded = false };

            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }
    }
}
