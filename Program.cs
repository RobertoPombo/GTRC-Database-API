using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Database_API.EfcContext;
using GTRC_Database_API.Models;
using GTRC_Database_API.Helpers;
using GTRC_Basics;

string pathSqlConnectionConfig = GlobalValues.DataDirectory + "config sqlConnection.json";
if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
SqlConnectionConfig sQLConCfg = JsonConvert.DeserializeObject<SqlConnectionConfig>(File.ReadAllText(pathSqlConnectionConfig, Encoding.Unicode)) ?? new();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(sQLConCfg.ConnectionString));

builder.Services.AddScoped(typeof(BaseService<>));
builder.Services.AddScoped(typeof(IBaseContext<>), typeof(BaseContext<>));

builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<ICarContext, CarContext>();

builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<ITrackContext, TrackContext>();


var app = builder.Build();

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
