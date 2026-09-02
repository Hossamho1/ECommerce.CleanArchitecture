<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.Filters;
namespace ECommerce.API.Filters;

public class AuditActionFilter(ILogger<AuditActionFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.FindFirst("Sub")?.Value;
        var actionName = context.ActionDescriptor.DisplayName;
        var executedContext = await next();
        logger.LogInformation("User {UserId} executed action {ActionName} with result {Result}", userId, actionName,
            executedContext.HttpContext.Response.StatusCode);

=======
﻿namespace ECommerce.API.Filters
{
    public class AuditActionFilter
    {
>>>>>>> 4f8d06f6baf74d5196ad533a476b1936b0454a60
    }
}
