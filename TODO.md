# Fix: Admin Page Shows NotFound Instead of Unauthorized

## Steps

- [x] 1. Configure `AccessDeniedPath` in `Program.cs` via `ConfigureApplicationCookie` to redirect to `/Error/Unauthorized`
- [x] 2. Fix `DashboardController.cs` — move `[Area("Admin")]` from action method to controller class level
