using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary;

namespace CRUDTestProject.Services.Authorization
{
    public class ServiceAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IEnumerable<string> claimValues;

        public ServiceAuthorizationFilter(IEnumerable<string> acceptAnyOf)
        {
            claimValues = acceptAnyOf;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.HasClaim(c => c.Type == Constants.ServiceClaimType && claimValues.Contains(c.Value)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
