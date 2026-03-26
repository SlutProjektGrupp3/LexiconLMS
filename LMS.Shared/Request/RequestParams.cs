using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Shared.Request
{
    public abstract class RequestParams
    {

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(2, 20)]
        public int PageSize { get; set; } = 2;

    }
    public class CourseRequestParams : RequestParams
    {
        public bool IncludeStudents { get; set; } = false;
    }
}
