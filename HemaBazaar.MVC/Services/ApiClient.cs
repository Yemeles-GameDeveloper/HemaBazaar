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

            var user = _contextAccessor.HttpContext!.User;

            string? token = await _tokenServices.GetValidTokenAsync(user);

            // Only attach the Authorization header when a valid token is available.
            // Public API endpoints (e.g. [AllowAnonymous]) will still work without it.
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
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
    }
}
