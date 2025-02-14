using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using System.Text;
using Scalar.AspNetCore;
using Sounds_New.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Sounds_New.Services.Users;
using Sounds_New.Services.Subscriptions;
using Sounds_New.Services.Tracks;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opts.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddOpenApi();
builder.Services.AddDbContext<SoundsContext>(options => options.UseSqlite("Data Source=sounds.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Localhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Audience"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<ISubscriptionsService, SubscriptionsService>();

var app = builder.Build();
app.UseCors("Localhost");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
