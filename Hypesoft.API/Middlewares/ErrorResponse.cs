using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.API.Middlewares
{
    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public object? Details { get; set; }
        public int StatusCode { get; set; }
    }

}