using BackEnd.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContext = await next();

           if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

           var userName = resultContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
           var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>(); 
           var user = await repo.GetUserByNameAsync(userName);
           user.LastActive = DateTime.Now;
           await repo.SaveAllAsync();
        }
    }
}
