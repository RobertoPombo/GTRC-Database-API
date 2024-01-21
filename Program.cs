using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

using GTRC_Basics;
using GTRC_Database_API.Data;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Database_API.EfcContext;

if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
string pathSqlConCfg = GlobalValues.DataDirectory + "config dbsql.json";
if (!File.Exists(pathSqlConCfg)) { File.WriteAllText(pathSqlConCfg, JsonConvert.SerializeObject(SqlConnectionConfig.List, Formatting.Indented), Encoding.Unicode); }
_ = JsonConvert.DeserializeObject<List<SqlConnectionConfig>>(File.ReadAllText(pathSqlConCfg, Encoding.Unicode)) ?? [];
if (SqlConnectionConfig.List.Count == 0) { _ = new SqlConnectionConfig(); }
SqlConnectionConfig? sqlConCfg = SqlConnectionConfig.GetActiveConnection();
if (sqlConCfg is null) { sqlConCfg = SqlConnectionConfig.List[0]; sqlConCfg.IsActive = true; }

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies().UseSqlServer(sqlConCfg.ConnectionString));

builder.Services.AddScoped(typeof(BaseService<>));
builder.Services.AddScoped(typeof(IBaseContext<>), typeof(BaseContext<>));

builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<IColorContext, ColorContext>();

builder.Services.AddScoped<SimService>();
builder.Services.AddScoped<ISimContext, SimContext>();

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

builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<IRoleContext, RoleContext>();

builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<IUserRoleContext, UserRoleContext>();

builder.Services.AddScoped<BopService>();
builder.Services.AddScoped<IBopContext, BopContext>();

builder.Services.AddScoped<BopTrackCarService>();
builder.Services.AddScoped<IBopTrackCarContext, BopTrackCarContext>();

builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<ISeriesContext, SeriesContext>();

builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<ISeasonContext, SeasonContext>();

builder.Services.AddScoped<SeasonCarclassService>();
builder.Services.AddScoped<ISeasonCarclassContext, SeasonCarclassContext>();

builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<IOrganizationContext, OrganizationContext>();

builder.Services.AddScoped<OrganizationUserService>();
builder.Services.AddScoped<IOrganizationUserContext, OrganizationUserContext>();

builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<ITeamContext, TeamContext>();

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<IEventContext, EventContext>();

builder.Services.AddScoped<EventCarclassService>();
builder.Services.AddScoped<IEventCarclassContext, EventCarclassContext>();

builder.Services.AddScoped<EventCarService>();
builder.Services.AddScoped<IEventCarContext, EventCarContext>();
/*
builder.Services.AddScoped<Service>();
builder.Services.AddScoped<IContext, Context>();
*/

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
