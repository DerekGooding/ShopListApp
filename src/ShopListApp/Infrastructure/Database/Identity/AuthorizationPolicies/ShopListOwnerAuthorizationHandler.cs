using ShopListApp.Responses;
using System.Security.Claims;

namespace ShopListApp.Infrastructure.Database.Identity.AuthorizationPolicies;

public class ShopListOwnerAuthorizationHandler : AuthorizationHandler<ShopListOwnerRequirement, ShopListResponse>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShopListOwnerRequirement requirement, ShopListResponse resource)
    {
        if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.OwnerId)
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
