using System;
using System.Linq;
using System.Collections.Generic;

namespace UOC.OrderService.Infrastructure.AspNet
{
    public class ApiResponseBase
    {
        public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
        public bool WaitForResponse { get; init; }
        public bool Processed { get; init; }
    }
    
    public class ApiResponseBase<T> :ApiResponseBase
    {
        public T Data { get; set; }
    }
}