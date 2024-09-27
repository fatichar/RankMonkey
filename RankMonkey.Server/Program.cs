using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RankMonkey.Server.Data;
using RankMonkey.Server.Exceptions;
using RankMonkey.Server.Services;
using RankMonkey.Server.Mapping;

using static Microsoft.AspNetCore.Http.StatusCodes;

// ... other using statements ...

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all network interfaces
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP port
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS port
    });
});

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(options =>
{
    string clientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new Exception("Google client ID not found");
    string clientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new Exception("Google client secret not found");
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.SaveTokens = true;
});

builder.Services.AddAuthorization();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string jwtKey = builder.Configuration["Jwt:SecretKey"] ?? throw new Exception("JWT secret key not found");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<IAuthProvider, GoogleAuthProvider>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<GuestAuthProvider, GuestAuthProvider>();
}

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<MetricsService>();
builder.Services.AddScoped<RankingService>();

// if (!builder.Environment.IsDevelopment())
// {
//     builder.Services.AddHttpsRedirection(options =>
//     {
//         options.RedirectStatusCode = Status308PermanentRedirect;
//         options.HttpsPort = 443;
//     });
// }

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RankMonkey API V1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RankMonkey API V1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

// Add CORS middleware
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();