using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Module
{
    public class DeleteModuleResultDto
    {
        public ModuleError? Error { get; set; } = null;

        public bool Succeeded { get; set; }

        public static DeleteModuleResultDto Success => new DeleteModuleResultDto { Succeeded = true };

        public static DeleteModuleResultDto Failed(ModuleError error)
        {
            var result = new DeleteModuleResultDto { Succeeded = false };
            if (error != null)
            {
                result.Error = error;
            }
            return result;
        }
    }
}
