using Microsoft.AspNetCore.Authorization;
using MusicStoreApi.Entities;
using System.Security.Claims;

namespace MusicStoreApi.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Artist>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Artist artist)
        {
            if (requirement.ResourceOperation == ResourceOperation.Read) // access to read(get) for user,premiumUser, admin
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userRule = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;

            // access to update, delete -> user, which created this artist and user: Admin
            if ( (requirement.ResourceOperation == ResourceOperation.Update || requirement.ResourceOperation == ResourceOperation.Delete) &&    
                (artist.CreatedById == int.Parse(userId) || userRule == "Admin") ) 
            {
                context.Succeed(requirement);
            }

            if (requirement.ResourceOperation == ResourceOperation.Create && 
                (userRule == "PremiumUser" || userRule == "Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
