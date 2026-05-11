using ITBudgeting.Infrastructure;
using ITBudgeting.Infrastructure.Data;
using ITBudgeting.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var euroCulture = new CultureInfo("de-DE");
    options.DefaultRequestCulture = new RequestCulture(euroCulture);
    options.SupportedCultures = [euroCulture];
    options.SupportedUICultures = [euroCulture];
});

var app = builder.Build();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BudgetingDbContext>();
    var hasAnyCostCenters = await db.CostCenters.AnyAsync();
    if (!hasAnyCostCenters)
    {
        db.CostCenters.AddRange(
            new CostCenter("91I40000", "BSS/OSS Operation"),
            new CostCenter("91I20000", "BSS/OSS DEV"),
            new CostCenter("91I10000", "Infrastructure"),
            new CostCenter("91I50000", "EPM and IT Governance"),
            new CostCenter("91I00000", "CIO office")
        );
        await db.SaveChangesAsync();
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
