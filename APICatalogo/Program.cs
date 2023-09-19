using Microsoft.EntityFrameworkCore;
using APICatalogo.Context;
using System.Text.Json.Serialization;
using APICatalogo.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. => ConfigureServices

builder.Services.AddControllers()
                .AddJsonOptions( options =>
                options.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.IgnoreCycles);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AppDbContext>(options =>
                            options.UseMySql(mySqlConnection,
                            ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build(); //Configure

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
