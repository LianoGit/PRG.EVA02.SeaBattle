using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;

var builder = WebApplication.CreateBuilder(args);
// hardcoded admin for school demo stuff
const string AdminEmail = "caekebeke.liano@gmail.com";

// register EF Core DbContext using connection string named "SeaBattle"
var conn = builder.Configuration.GetConnectionString("SeaBattle");
builder.Services.AddDbContext<SeaBattleDbContext>(options =>
    options.UseSqlServer(conn));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ISeaBattleService, SeaBattleService>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "seaBattleThrowBomb",
    pattern: "SeaBattle/{gameId:int}/ThrowBomb/{letter?}/{number?}",
    defaults: new { controller = "SeaBattle", action = "ThrowBomb" }
);

app.MapControllerRoute(
    name: "seaBattleThrowBombs",
    pattern: "SeaBattle/{gameId:int}/ThrowBombs/{letter?}/{number?}",
    defaults: new { controller = "SeaBattle", action = "ThrowBomb" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await SeedAdminUserAsync(app.Services, AdminEmail);

app.Run();

static async Task SeedAdminUserAsync(IServiceProvider services, string adminEmail)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SeaBattleDbContext>();
    var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<AppUser>();

    if (!await context.Database.CanConnectAsync())
    {
        // db maybe offline, just skip and move on
        return;
    }

    var usersTableCount = await context.Database
        .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'")
        .SingleAsync();

    var usersTableExists = usersTableCount > 0;

    if (!usersTableExists)
    {
        // during fresh migrate this table isnt there yet
        return;
    }

    var normalizedEmail = adminEmail.Trim().ToUpperInvariant();
    var adminUser = await context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

    if (adminUser == null)
    {
        // quick bootstrap user so i dont get locked out
        adminUser = new AppUser
        {
            Email = adminEmail,
            NormalizedEmail = normalizedEmail,
            Role = "Administrator"
        };

        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin");
        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
        return;
    }

    if (!string.Equals(adminUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
    {
        adminUser.Role = "Administrator";
        await context.SaveChangesAsync();
    }
}
