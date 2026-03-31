using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Course
{
    public record ParticipantDto(Guid Id, string FirstName, string LastName, string Email);
};