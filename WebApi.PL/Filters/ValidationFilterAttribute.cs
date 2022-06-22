using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.PL.Filters
{
    /* If the model state is not valid, return a bad request with the model state errors */
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage));
                context.Result = new BadRequestObjectResult(errors);
            }
        }
    }
}
