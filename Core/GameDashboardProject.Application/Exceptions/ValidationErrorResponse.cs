using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDashboardProject.Application.Exceptions
{
    public class ValidationErrorResponse
    {
        public string Title { get; set; } = "Validation Errors"; 
        public int Status { get; set; } 
        public List<ValidationError> Errors { get; set; } = new(); 
    }
}
