namespace HemaBazaar.MVC.Middlewares
{
    public class CustomErrorMiddleware
    {
        readonly RequestDelegate _next;

        public CustomErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.Redirect("/Error/NotFound");
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Redirect("/Error/Unauthorized");
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.Redirect("/Error/Forbidden");
            }

        }
    }
}
