using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Services;

var builder = WebApplication.CreateBuilder(args);

// register EF Core DbContext using connection string named "SeaBattle"
var conn = builder.Configuration.GetConnectionString("SeaBattle");
builder.Services.AddDbContext<SeaBattleDbContext>(options =>
    options.UseSqlServer(conn));

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

app.Run();
