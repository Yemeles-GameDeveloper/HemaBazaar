# Fix Redis Issue & Rebuild Auth/Authorization

## Steps

### Issue 1 — Redis Fix
- [x] 1. Fix `IConnectionMultiplexer` registration in `HemaBazaar.MVC/Program.cs`: use `redisConfig` section + `abortConnect=false`

### Issue 2 — Auth/Authorization Rebuild

#### API Side
- [x] 2. Add `builder.Services.Configure<JwtSettings>(...)` in `HemaBazaar.API/Program.cs`
- [x] 3. Add `[Route("api/[controller]")]` and `[ApiController]` to `AuthController.cs` in API
- [x] 4. Fix missing space in `JwtCookieMiddleware.cs`: `"Bearer " + token`

#### MVC Side
- [x] 5. Remove standalone `builder.Services.AddAuthentication()` from `HemaBazaar.MVC/Program.cs`
- [x] 6. Move `app.UseSession()` before `app.UseRouting()` in `HemaBazaar.MVC/Program.cs`
- [x] 7. Fix `TokenServices.cs` — removed broken `RefreshTokenAsync`; added `StoreToken()` and `ClearToken()` helpers; `GetValidTokenAsync` returns `null` when expired
- [x] 8. Fix `AccountController.cs` — after Identity sign-in, call API to get JWT and store in session via `FetchAndStoreApiTokenAsync`; clear session token on logout
- [x] 9. Fix `ApiClient.cs` — handle `null` token from `GetValidTokenAsync`; use `ApiBaseUrl` from config instead of hardcoded URL
