using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelayChat_Identity.Models;
using RelayChat_Identity.Services;
using WebAuthn.Net.Storage.SqlServer.Configuration.DependencyInjection;

var webBuilder = WebApplication.CreateBuilder(args);

webBuilder.Services
    .AddControllersWithViews()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

webBuilder.Services.AddSwaggerGen();

webBuilder.Services.AddDbContext<RelayChatIdentityContext>(
    options =>
        options.UseSqlServer(
            webBuilder.Configuration.GetConnectionString("DbContext"),
            providerOptions => providerOptions.EnableRetryOnFailure()
        )
);

webBuilder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<RelayChatIdentityContext>();

webBuilder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

webBuilder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Password.RequiredLength = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});

webBuilder.Services.AddWebAuthnSqlServer(configureSqlServer: sqlServer =>
{
    sqlServer.ConnectionString = webBuilder.Configuration.GetConnectionString("DbContext") ??
                                 throw new Exception("DB CONNECTION STRING NOT FOUND!");
});

webBuilder.Services.AddScoped<AppSettings>();
webBuilder.Services.AddScoped<RegistrationCeremonyHandleService>();
webBuilder.Services.AddScoped<AuthenticationCeremonyHandleService>();

var app = webBuilder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.UsePathBase("/api");
app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();