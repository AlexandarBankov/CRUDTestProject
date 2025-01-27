using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Services.Authorization
{
    public class ServiceAuthorizationAttribute : TypeFilterAttribute
    {
        public ServiceAuthorizationAttribute(params string[] acceptAnyOf) : base(typeof(ServiceAuthorizationFilter))
        {
            Arguments = [acceptAnyOf.ToList()];
        }
    }
}
