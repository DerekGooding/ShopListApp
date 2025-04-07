using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopListApp.API;

public class QuantityFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("quantity", out var value) && value is int)
        {
            var quantity = (int)value;
            if (quantity < 1)
                context.Result = new BadRequestObjectResult("Quantity must be greater than 0.");
        }
    }
}
