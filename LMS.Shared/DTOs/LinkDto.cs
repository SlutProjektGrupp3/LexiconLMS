using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs
{
    public class LinkDto
    {
        public string Href { get; set; } = default!;
        public string Rel { get; set; } = default!;
        public string Method { get; set; } = default!;
    }
}
