using Shoppers.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the Database Context with SQL Server connection
builder.Services.AddDbContext<ShoppersDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Configure Routing Options
// Enforces lowercase URLs for better SEO (e.g., /home/index instead of /Home/Index)
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

// Database Initialization Scope
// This block runs every time the application starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShoppersDbContext>();

    // ⚠️ DEVELOPMENT ONLY: Deletes the existing database to reset state.
    // This ensures that any changes to Seed Data (like adding products) are applied.
    // Remove this line when moving to Production or using Migrations.
    db.Database.EnsureDeleted();

    // Creates the database if it doesn't exist and applies Seed Data.
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map default controller route pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();