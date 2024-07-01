
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.API.Filters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;
            // Customize the response based on validation errors
            var errors = context.ModelState.Keys
                .Select(key => new ErrorModelState(key, context.ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()));

            var response = new ResponseModel<IEnumerable<ErrorModelState>>
            {
                StatusCode = 400,
                Message = "Invalid Input",
                Body = errors
            };
            context.Result = new BadRequestObjectResult(response);
        }
    }
}
