using Microsoft.AspNetCore.Authorization;

namespace MusicStoreApi.Authorization
{
    public enum ResourceOperation
    {
        Create,
        Read,
        Update,
        Delete
    }

    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(ResourceOperation _resourceOperation)
        {
            ResourceOperation = _resourceOperation;
        }

        public ResourceOperation ResourceOperation { get; set; }
    }
}
