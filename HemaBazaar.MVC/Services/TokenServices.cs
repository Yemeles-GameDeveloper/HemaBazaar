using HemaBazaar.MVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HemaBazaar.MVC.Services
{
    public class TokenServices
    {
        IHttpContextAccessor _contextAccessor;
        IConfiguration _configuration;

        public TokenServices(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Returns the stored JWT from session if it is still valid; otherwise returns null.
        /// Callers should redirect to login when null is returned.
        /// </summary>
        public Task<string?> GetValidTokenAsync(ClaimsPrincipal user)
        {
            var httpContext = _contextAccessor.HttpContext!;

            // 1) Primary source: session
            var token = httpContext.Session.GetString("access_token");
            if (IsTokenValid(token))
                return Task.FromResult<string?>(token);

            // 2) Fallback source: request cookie
            var cookieToken = httpContext.Request.Cookies["access_token"];
            if (IsTokenValid(cookieToken))
            {
                // Self-heal: restore session token when cookie is valid.
                httpContext.Session.SetString("access_token", cookieToken!);
                return Task.FromResult<string?>(cookieToken);
            }

            // Token is expired or missing — caller must redirect to login.
            return Task.FromResult<string?>(null);
        }

        /// <summary>
        /// Stores a JWT token in the session after a successful login.
        /// </summary>
        public void StoreToken(string token)
        {
            _contextAccessor.HttpContext!.Session.SetString("access_token", token);
        }

        /// <summary>
        /// Removes the JWT token from the session on logout.
        /// </summary>
        public void ClearToken()
        {
            _contextAccessor.HttpContext!.Session.Remove("access_token");
        }

        public bool IsTokenValid(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    return false;

                var jwt = handler.ReadJwtToken(token);

                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        // 4 Kasım 0:51:00
    }
}
