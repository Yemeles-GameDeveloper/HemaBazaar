namespace HemaBazaar.API.Middlewares
{
    public class JwtCookieMiddleware
    {
        RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if(!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Authorization = "Bearer " + token;
            }

            await _next(context);
        }
    }
}
