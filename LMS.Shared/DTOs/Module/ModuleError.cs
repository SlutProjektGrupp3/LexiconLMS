
namespace LMS.Shared.DTOs.Module;

public class ModuleError
{
    public string Code { get; set; } = "MODULE_ERROR";
    public string Description { get; set; } = "Something bad happened";

    public ErrorStatusCode StatusCode { get; set; } = ErrorStatusCode.Database;
}
