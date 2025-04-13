using Simpled.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Simpled.Hubs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Simpled.Repository;
using Simpled.Services;
using System.Reflection;
using Simpled.Exception;
using Microsoft.AspNetCore.SignalR;
using Simpled.Helpers;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
//  DbContext 
// --------------------------------------------------
builder.Services.AddDbContext<SimpledDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// --------------------------------------------------
//  JWT
// --------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"].FirstOrDefault();
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/board"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


// --------------------------------------------------
//  CORS
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // o el dominio de Vercel
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --------------------------------------------------
//  Controladores + FluentValidation
// --------------------------------------------------
builder.Services.AddControllers()
    .AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --------------------------------------------------
//  Swagger + Bearer Authorization
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header usando el esquema Bearer. \r\n\r\nIntroduce 'Bearer <TOKEN>' para autenticarte."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Incluir comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// --------------------------------------------------
//  SignalR
// --------------------------------------------------
builder.Services.AddSignalR();

// --------------------------------------------------
//  Authorization
// --------------------------------------------------
builder.Services.AddAuthorization();

// --------------------------------------------------
//  Servicios y Repositorios
// --------------------------------------------------
builder.Services.AddScoped<IAuthRepository, AuthService>();
builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddScoped<IBoardRepository, BoardService>();
builder.Services.AddScoped<IColumnRepository, ColumnService>();
builder.Services.AddScoped<IItemRepository, ItemService>();
builder.Services.AddScoped<IBoardMemberRepository, BoardMemberService>();
builder.Services.AddScoped<IBoardInvitationRepository, BoardInvitationService>();
builder.Services.AddScoped<AchievementsService>();
builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdHelper>();






var app = builder.Build();

// --------------------------------------------------
//  Crear DB si no existe
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SimpledDbContext>();
    db.Database.EnsureCreated();
}

// --------------------------------------------------
//  Middlewares
// --------------------------------------------------
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BoardHub>("/hubs/board");

app.Run();
