using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Shared.DTOs.Activity
{
    public record CreateActivityDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Activity Type is required")]
        public string ActivityTypeName { get; set; } = string.Empty;

        public Guid ModuleId { get; set; }
    }
}
