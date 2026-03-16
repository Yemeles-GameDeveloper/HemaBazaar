﻿using HemaBazaar.MVC.Models;

namespace HemaBazaar.MVC.Services
{
    public class ApiClient
    {
        IHttpClientFactory _factory;
        TokenServices _tokenServices;
        IHttpContextAccessor _contextAccessor;
        IConfiguration _configuration;

        public ApiClient(IConfiguration configuration, IHttpClientFactory factory, TokenServices tokenServices, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _factory = factory;
            _tokenServices = tokenServices;
            _contextAccessor = contextAccessor;
        }

        // Ensures BaseAddress ends with '/' and the relative url does NOT start with '/'
        // so HttpClient resolves: "https://host/api/" + "Item" = "https://host/api/Item"
        static string NormalizeUrl(string url) => url.TrimStart('/');

        async Task<HttpClient> CreateClient()
        {
            var client = _factory.CreateClient();

            // BaseAddress MUST end with a trailing slash for relative URLs to resolve correctly
            var baseUrl = _configuration["ApiBaseUrl"]!.TrimEnd('/') + "/api/";
            client.BaseAddress = new Uri(baseUrl);

            var httpContext = _contextAccessor.HttpContext;
            var user = httpContext!.User;

            string? token = await _tokenServices.GetValidTokenAsync(user);

            // Fallback: if session token is missing/expired, try auth cookie token.
            if (string.IsNullOrWhiteSpace(token))
            {
                token = httpContext.Request.Cookies["access_token"];
            }

            // On-demand bootstrap: if still missing but user is authenticated in MVC,
            // request a fresh API token from identity and persist it.
            if (string.IsNullOrWhiteSpace(token) && (user?.Identity?.IsAuthenticated ?? false))
            {
                token = await BootstrapTokenFromApiAsync(client, httpContext);
            }

            // Attach Authorization header whenever we have a token string.
            // Let API-side JWT validation be the source of truth.
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        async Task<string?> BootstrapTokenFromApiAsync(HttpClient client, HttpContext httpContext)
        {
            try
            {
                // Forward incoming cookies explicitly so API [Authorize] can resolve current identity.
                var request = new HttpRequestMessage(HttpMethod.Post, "Auth/token-from-user");
                if (httpContext.Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    request.Headers.TryAddWithoutValidation("Cookie", cookieHeaderValues.ToString());
                }

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;

                var tokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponseModel>(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                var token = tokenResponse?.Token;

                // Fallback: parse token from Set-Cookie if body token is empty.
                if (string.IsNullOrWhiteSpace(token))
                {
                    if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
                    {
                        foreach (var header in setCookieHeaders)
                        {
                            const string key = "access_token=";
                            var start = header.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                            if (start < 0) continue;

                            start += key.Length;
                            var end = header.IndexOf(';', start);
                            token = end > start ? header[start..end] : header[start..];
                            if (!string.IsNullOrWhiteSpace(token))
                                break;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(token))
                    return null;

                httpContext.Session.SetString("access_token", token);
                httpContext.Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = httpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = tokenResponse?.ExpireDate == default
                        ? DateTimeOffset.UtcNow.AddMinutes(60)
                        : tokenResponse!.ExpireDate
                });

                return token;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var client = await CreateClient();
            var response = await client.GetAsync(NormalizeUrl(url));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> PostAsync<TData, TResponse>(string url, TData data)
        {
            var client = await CreateClient();
            var response = await client.PostAsJsonAsync(NormalizeUrl(url), data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HemaBazaar.MVC.Models.HttpResponse> PostStatusAsync<TData>(string url, TData data)
        {
            var client = await CreateClient();
            var response = await client.PostAsJsonAsync(NormalizeUrl(url), data);
            var content = await response.Content.ReadAsStringAsync();

            return new HemaBazaar.MVC.Models.HttpResponse
            {
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Content = content
            };
        }

        public async Task<TResponse> PutAsync<TData, TResponse>(string url, TData data)
        {
            var client = await CreateClient();
            var response = await client.PutAsJsonAsync(NormalizeUrl(url), data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            var client = await CreateClient();
            var response = await client.DeleteAsync(NormalizeUrl(url));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }



        //3 Aralık 1:38:00dan devam et.
    }
}
