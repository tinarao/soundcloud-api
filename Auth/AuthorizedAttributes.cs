using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sounds_New.Models;
using System;
using System.Linq;
using System.Security.Claims;

public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly UserRoles _role;

    public CustomAuthorizeAttribute(UserRoles role)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRole = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (userRole == null || !Enum.TryParse(userRole, out UserRoles role) || role != _role)
        {
            context.Result = new ForbidResult();
        }
    }
}