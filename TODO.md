# TODO - Fix cart add token null issue

- [x] Update `HemaBazaar.MVC/Services/ApiClient.cs` to fallback to cookie token when session token is null.
- [x] Remove local token-validity gate in `ApiClient` and always attach Bearer when token exists.
- [x] Harden API token persistence in `HemaBazaar.MVC/Controllers/AccountController.cs` (JSON token first, Set-Cookie fallback).
- [ ] Build solution to verify compile success.
- [ ] Mark tasks complete.
