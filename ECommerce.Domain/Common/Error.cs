using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Common;

public sealed record Error(string Code ,string Message ,ErrorType Type)
{
    public static Error Validation(string code, string message) => new Error(code, message, ErrorType.Validation); 

    public static Error NotFound(string code, string message) => new Error(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new Error(code, message, ErrorType.Conflict);
    public static Error Unauthorized(string code, string message) => new Error(code, message, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string message) => new Error(code, message, ErrorType.Forbidden);
    public static Error Failure(string code, string message) => new Error(code, message, ErrorType.Failure);

}
