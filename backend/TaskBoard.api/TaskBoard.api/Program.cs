using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskBoard.api.Data;
using TaskBoard.api.Filters;
using TaskBoard.api.Hubs;
using TaskBoard.api.Mapping;
using TaskBoard.api.Models;
using TaskBoard.api.Services;
using TaskBoard.api.Utils;
using AutoMapper;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Text.Json;

// Limpia el mapeo de claims por defecto
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskBoard API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insertar JWT con 'Bearer '",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // No usamos MapInboundClaims = false; usamos el Clear al inicio
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"OnAuthenticationFailed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validado correctamente");
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,

            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"))
    .AddOptions<JwtSettings>()
    .ValidateDataAnnotations();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

builder.Services.AddScoped<UserRolesResolver>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ActivityLogger>();
builder.Services.AddSignalR();
builder.Services.AddScoped<BoardAccessFilter>();
builder.Services.AddScoped<IColumnService, ColumnService>();

// Registrar el servicio de autenticación personalizado
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BoardAccess", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("BoardAdmin", policy => policy.RequireRole("Admin"));
});

// Registrar AutoMapper y el MappingProfile
builder.Services.AddAutoMapper((provider, cfg) =>
{
    cfg.ConstructServicesUsing(type => provider.GetRequiredService(type));
    cfg.AddProfile(new MappingProfile());
}, typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net8.0");
    var fullPath = Path.Combine(path, "TaskBoard.db");
    options.UseSqlite($"Data Source={fullPath}");
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
});

// Configuración CORS para SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", builder =>
    {
        builder.WithOrigins("https://example.com", "https://another-example.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "User", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
    Console.WriteLine($"Database created at: {Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net8.0/TaskBoard.db")}");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("SignalRPolicy");
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Authenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"Claims count: {context.User?.Claims.Count()}");
    await next();
});
app.UseAuthorization();
app.MapHub<BoardHub>("/hubs/boards");
app.MapControllers();
app.Run();
