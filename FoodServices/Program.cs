using FoodServices.Model;
using FoodServices.Services;
using FoodServices.Setting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();   
builder.Services.AddDbContextPool<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Settings
builder.Services.Configure<LoginSetting>(builder.Configuration.GetSection("LoginSetting"));
builder.Services.Configure<ServiceConfig>(builder.Configuration.GetSection("ServiceConfig"));

// Worker
builder.Services.AddHostedService<FoodWorker>();



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
