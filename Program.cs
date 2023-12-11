using biometricService.Data;
using biometricService.Http;
using biometricService.Interfaces;
using biometricService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("InnovatricsApi", client =>
{
    client.BaseAddress = new Uri("https://dot.innovatrics.com");
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();     
    }
    ));

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
