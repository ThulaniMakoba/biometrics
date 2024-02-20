using biometricService.Data;
using biometricService.Data.Implementation;
using biometricService.Data.Interfaces;
using biometricService.Http;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy(name: "eDNACors",
    policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(p => true);
    }));

builder.Configuration.AddJsonFile("appsettings.json");

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);

builder.Services.AddSingleton(new LdapService($"LDAP://{appSettings.DomainName}"));

builder.Services.AddHttpClient();
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IInnovatricsService, InnovatricsService>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFaceDataRepository, FaceDataRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }
    ));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("eDNACors");
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
