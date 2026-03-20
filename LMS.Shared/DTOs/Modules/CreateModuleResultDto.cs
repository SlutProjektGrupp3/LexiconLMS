namespace LMS.Shared.DTOs.Modules
{
    public class CreateModuleResultDto
    {
        private static readonly CreateModuleResultDto _success = new CreateModuleResultDto { Succeeded = true };
        private readonly List<ModuleError> _errors = new List<ModuleError>();

        public bool Succeeded { get; protected set; }

        public IEnumerable<ModuleError> Errors => _errors;

        public static CreateModuleResultDto Success => _success;

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
