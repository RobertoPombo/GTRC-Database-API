using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Database_API.EfcContext;
using GTRC_Database_API.Helpers;

SqlConnectionConfig sQLConCfg = Basics.GetSqlConnection();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies().UseSqlServer(sQLConCfg.ConnectionString));

builder.Services.AddScoped(typeof(BaseService<>));
builder.Services.AddScoped(typeof(IBaseContext<>), typeof(BaseContext<>));

builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<IColorContext, ColorContext>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<ITrackContext, TrackContext>();

builder.Services.AddScoped<CarclassService>();
builder.Services.AddScoped<ICarclassContext, CarclassContext>();

builder.Services.AddScoped<ManufacturerService>();
builder.Services.AddScoped<IManufacturerContext, ManufacturerContext>();

builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<ICarContext, CarContext>();

builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<ISeriesContext, SeriesContext>();

builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<ISeasonContext, SeasonContext>();

builder.Services.AddScoped<SeasonCarclassService>();
builder.Services.AddScoped<ISeasonCarclassContext, SeasonCarclassContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//GTRC_Database_API.Helpers.HostBuilder.CreateHostBuilder(args).Build().Run();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
