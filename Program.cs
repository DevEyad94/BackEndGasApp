global using BackEndGasApp.Data;
global using BackEndGasApp.Extensions;
global using BackEndGasApp.Extensions.Pagination;
global using BackEndGasApp.Models;
global using BackEndGasApp.Models.zsk;
global using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BackEndGasApp.Constants;
using BackEndGasApp.Converters;
using BackEndGasApp.Helpers;
using BackEndGasApp.Services.DatabaseService;
using BackEndGasApp.Services.DashboardService;
using BackEndGasApp.Services.GasService;
using BackEndGasApp.Services.UserService;
using BackEndGasApp.Services.ZskService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddControllers();

// Add this for explicit API explorer configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Gas App API";
    options.Version = "v1";
    options.Description = "An .NET Core Web API for managing gas app data";

    // Add explicit configuration for controllers
    options.ApiGroupNames = new[] { "v1" };

    options.PostProcess = document =>
    {
        // Debug information at startup
        Console.WriteLine($"OpenAPI document generated with {document.Paths.Count} paths");

        document.Info = new NSwag.OpenApiInfo
        {
            Version = "v1",
            Title = "Gas App API",
            Description = "An .NET Core Web API for managing gas app data",
            TermsOfService = "https://example.com/terms",
            Contact = new NSwag.OpenApiContact
            {
                Name = "Example Contact",
                Url = "https://example.com/contact",
            },
            License = new NSwag.OpenApiLicense
            {
                Name = "Example License",
                Url = "https://example.com/license",
            },
        };
        document.SecurityDefinitions.Add(
            "bearerAuth",
            new NSwag.OpenApiSecurityScheme
            {
                Type = NSwag.OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
            }
        );
        document.Security.Add(
            new NSwag.OpenApiSecurityRequirement { { "bearerAuth", Array.Empty<string>() } }
        );
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["AppSettings:Token"]
                        ?? throw new InvalidOperationException("JWT Token is not configured")
                )
            ),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminPolicy, policy => policy.RequireRole("Admin"));
    options.AddPolicy(Policies.EngineerPolicy, policy => policy.RequireRole("Engineer"));
    options.AddPolicy(Policies.OperatorPolicy, policy => policy.RequireRole("Operator"));
    options.AddPolicy(
        Policies.AdminEngineerPolicy,
        policy =>
        {
            policy.RequireRole("Admin", "Engineer");
        }
    );
    options.AddPolicy(
        Policies.AdminOperatorPolicy,
        policy =>
        {
            policy.RequireRole("Admin", "Operator");
        }
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        }
    );
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IZskService, ZskService>();
builder.Services.AddScoped<IGasService, GasService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<AutoMapperProfiles>();
});
builder
    .Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        x.JsonSerializerOptions.Converters.Add(new DateTimeNonNullableConverter());
    });

// Register AdminOnlyConverterFactory after HttpContextAccessor and JsonOptions are configured
builder.Services.AddSingleton<AdminOnlyConverterFactory>(provider => new AdminOnlyConverterFactory(
    provider.GetRequiredService<IHttpContextAccessor>()
));

// Set the environment in your Constants class
BackEndGasApp.Constants.Constant.SetEnvironment(builder.Environment.EnvironmentName);

var app = builder.Build();

Console.WriteLine("Current environment: " + app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    Console.WriteLine("Configuring OpenAPI and Scalar...");

    // OpenAPI configuration - make available in any environment
    app.UseOpenApi(options =>
    {
        options.Path = "/openapi/{documentName}.json";
        Console.WriteLine("OpenAPI path configured: " + options.Path);
    });

    // Configure Scalar API reference
    app.MapScalarApiReference(options =>
    {
        options.Title = "Project API Documentation";
        options.EndpointPathPrefix = "/scalar/{documentName}";
        // Ensure the OpenAPI path matches the one set above
        options.OpenApiRoutePattern = "/openapi/{documentName}.json";
        Console.WriteLine("Scalar configured at: " + options.EndpointPathPrefix);
    });
}

app.UseHttpsRedirection();

// First serve the static files from wwwroot
app.UseStaticFiles();

// Then add specific static files middleware to serve browser folder at root URL
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "browser")
        ),
        RequestPath = "",
    }
);

app.UseCors(MyAllowSpecificOrigins);

// app.UseMvc();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback route for SPA
app.MapFallbackToFile("browser/index.html");

// Add a debug endpoint to check if OpenAPI is generating properly
app.MapGet(
    "/debug-openapi",
    async (HttpContext context) =>
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Debug OpenAPI Information:\n");
        await context.Response.WriteAsync($"Environment: {app.Environment.EnvironmentName}\n");
        await context.Response.WriteAsync($"IsDevelopment: {app.Environment.IsDevelopment()}\n");
        await context.Response.WriteAsync($"OpenAPI document should be at: /openapi/v1.json\n");
        await context.Response.WriteAsync($"Scalar UI should be at: /scalar/v1\n");
        await context.Response.WriteAsync("Try accessing these URLs directly.");
    }
);

app.Run();
