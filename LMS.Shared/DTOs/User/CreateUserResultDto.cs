
namespace LMS.Shared.DTOs.User;

public class CreateUserResultDto
{
    private readonly List<UserError> _errors = new List<UserError>();

    public bool Succeeded { get; set; }

    public IEnumerable<UserError> Errors => _errors;

    // Return a fresh success instance to avoid mutating a shared singleton
    public static CreateUserResultDto Success => new CreateUserResultDto { Succeeded = true };

    public UserDto? CreatedUser { get; set; } = null;

    // Factory method to create a success result that includes the created user
    public static CreateUserResultDto SuccessWith(UserDto user) =>
        new CreateUserResultDto { Succeeded = true, CreatedUser = user };

    public static CreateUserResultDto Failed(List<UserError>? errors)
    {
        var result = new CreateUserResultDto { Succeeded = false };

        if (errors != null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    } 
}
