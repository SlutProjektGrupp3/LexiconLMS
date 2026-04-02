using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.User
{
    public record AvailableStudentDto(
    string Id,
    string FirstName,
    string LastName,
    string Email
);
}
