using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs
{
    public record ActivityDto(
    Guid Id,
    string Name,
    string TypeName, 
    string Description,
    DateTime StartDate,
    DateTime EndDate
    );
}
