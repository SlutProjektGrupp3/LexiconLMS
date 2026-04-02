namespace LMS.Shared.DTOs.Modules
{
    public class ModuleError
    {
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;

        public ErrorStatusCode StatusCode { get; set; } = ErrorStatusCode.Database;
    }
}
