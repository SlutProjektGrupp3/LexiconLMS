using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Activity
{
    public record UpdateActivityDto(
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate,
        Guid TypeId
    );
}
