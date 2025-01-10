using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDashboardProject.Application.Exceptions
{
    public class ValidationError
    {
        public string PropertyName { get; set; } 
        public string ErrorMessage { get; set; } 
    }
}
