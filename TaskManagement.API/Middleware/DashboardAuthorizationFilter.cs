using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.Security.Claims;

namespace TaskManagement.API.Middleware;

public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var userRole = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return httpContext.User.Identity?.IsAuthenticated ?? false && userRole == ConstRoles.Admin;
    }   
    
}
