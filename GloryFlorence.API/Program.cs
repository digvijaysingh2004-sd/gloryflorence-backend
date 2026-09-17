using GloryFlorence.API.Middleware;
using GloryFlorence.Application;
using GloryFlorence.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings.GetValue<string>("Secret") ?? throw new InvalidOperationException("JWT Secret is not configured.");
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer") ?? "GloryFlorenceAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings.GetValue<string>("Audience") ?? "GloryFlorenceClient",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new GloryFlorence.Application.Common.Converters.FlexibleStringJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new GloryFlorence.Application.Common.Converters.FlexibleTimeSpanJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new GloryFlorence.Application.Common.Converters.NullableFlexibleTimeSpanJsonConverter());
    });

// Configure CORS for React frontend (supports standard port 3000 and Vite port 5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins(
                  "http://localhost:3000", "https://localhost:3000",
                  "http://localhost:5173", "https://localhost:5173",
                  "http://127.0.0.1:3000", "https://127.0.0.1:3000",
                  "http://127.0.0.1:5173", "https://127.0.0.1:5173"
              )
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Glory Florence Physiotherapy Management API",
        Version = "v1",
        Description = "Backend API for managing Glory Florence Physiotherapy clinic."
    });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token directly."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Handle command-line user-only seed execution
if (args.Contains("--seed-users-only"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Starting User-only database seeding...");
        var context = services.GetRequiredService<GloryFlorence.Infrastructure.Data.ApplicationDbContext>();
        await GloryFlorence.Infrastructure.Data.ApplicationDbContextSeed.SeedUsersOnlyAsync(context);
        logger.LogInformation("User table seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during user table seeding.");
        Environment.ExitCode = 1;
    }
    return;
}

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GloryFlorence.Infrastructure.Data.ApplicationDbContext>();
        await GloryFlorence.Infrastructure.Data.ApplicationDbContextSeed.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// Use global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Glory Florence API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
