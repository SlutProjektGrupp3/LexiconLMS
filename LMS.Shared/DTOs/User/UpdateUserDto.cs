
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Shared.DTOs.User;

public class UpdateUserDto : BaseUserDto
{
    [Required(ErrorMessage = "User ID is required for updating.")]
    public string Id { get; set; } = "";
}
