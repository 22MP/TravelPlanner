using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Filters
{
    public class SharedLinkAccessFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Ako nema X-Share-Token headera, normalan JWT tok
            if (!context.HttpContext.Request.Headers
                .TryGetValue("X-Share-Token", out var token))
            {
                await next();
                return;
            }

            var method = context.HttpContext.Request.Method;
            var requiredAccess = method == "GET" ? "VIEW" : "EDIT";

            var sharedLinkServices = ServiceProxy.Create<ISharedLinkServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));

            var isValid = await sharedLinkServices
                .ValidateTokenAsync(token!, requiredAccess);

            if (!isValid)
            {
                context.Result = new ObjectResult(
                    new { message = "Invalid, expired, or insufficient access." })
                {
                    StatusCode = 403
                };
                return;
            }

            await next();
        }
    }
}