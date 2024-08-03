using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RelayChat_Identity.Models;

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

var app = webBuilder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();