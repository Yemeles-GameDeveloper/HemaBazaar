using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public static Result<T> Ok(T data, string message = "") => new Result<T> 
        { Success = true, Message = message, Data = data };
        public static Result<T> Failure( string message) => new Result<T>
        { Success = false, Message = message };
    }
}
