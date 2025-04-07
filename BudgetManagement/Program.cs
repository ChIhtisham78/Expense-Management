using ExpenseManagment.Custom.DataSeeding;
using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IAuthenticatedUserAccessor, AuthenticatedUserAccessor>();

builder.Services.
    AddControllersWithViews().
    AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).
           AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()).
           AddRazorRuntimeCompilation();
AppConfiguration.Configure<AuditSettings>(builder.Services, builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));
});


var app = builder.Build();

var logName = "default";
var assemblyLoc = Assembly.GetExecutingAssembly().Location;
var assemblyDir = Path.GetDirectoryName(assemblyLoc);

var logDirectory = Path.Combine(assemblyDir, "log");

if (!Directory.Exists(logDirectory))
{
    Console.WriteLine($"Log directory does not exist. Creating: {logDirectory}");
    Directory.CreateDirectory(logDirectory);
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logDirectory, $"{logName}_{DateTime.UtcNow.ToString("MM-dd-yyyy")}.log"),
        rollingInterval: RollingInterval.Infinite)
    .CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    var _roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
    var _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = service.GetRequiredService<ApplicationDbContext>(); // Get the ApplicationDbContext

    IdentitySeed.SeedData(_userManager, _roleManager);
    DefaultSeeding.seedDefaultAccounts(dbContext);
}
app.Run();
