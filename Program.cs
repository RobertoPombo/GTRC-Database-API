using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Database_API.EfcContext;
using GTRC_Basics.Configs;



SqlConnectionConfig.LoadJson();
SqlConnectionConfig? sqlConCfg = SqlConnectionConfig.GetActiveConnection();
if (sqlConCfg is null) { sqlConCfg = SqlConnectionConfig.List[0]; sqlConCfg.IsActive = true; }

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies().UseSqlServer(sqlConCfg.ConnectionString));

builder.Services.AddScoped(typeof(BaseService<>));
builder.Services.AddScoped(typeof(IBaseContext<>), typeof(BaseContext<>));
builder.Services.AddScoped(typeof(FullService<>));

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

builder.Services.AddScoped<UserDatetimeService>();
builder.Services.AddScoped<IUserDatetimeContext, UserDatetimeContext>();

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

builder.Services.AddScoped<EntryService>();
builder.Services.AddScoped<IEntryContext, EntryContext>();

builder.Services.AddScoped<EntryDatetimeService>();
builder.Services.AddScoped<IEntryDatetimeContext, EntryDatetimeContext>();

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<IEventContext, EventContext>();

builder.Services.AddScoped<EventCarclassService>();
builder.Services.AddScoped<IEventCarclassContext, EventCarclassContext>();

builder.Services.AddScoped<EventCarService>();
builder.Services.AddScoped<IEventCarContext, EventCarContext>();

builder.Services.AddScoped<EntryEventService>();
builder.Services.AddScoped<IEntryEventContext, EntryEventContext>();

builder.Services.AddScoped<EntryUserEventService>();
builder.Services.AddScoped<IEntryUserEventContext, EntryUserEventContext>();

builder.Services.AddScoped<PointssystemService>();
builder.Services.AddScoped<IPointssystemContext, PointssystemContext>();

builder.Services.AddScoped<PointssystemPositionService>();
builder.Services.AddScoped<IPointssystemPositionContext, PointssystemPositionContext>();

builder.Services.AddScoped<StintAnalysisMethodService>();
builder.Services.AddScoped<IStintAnalysisMethodContext, StintAnalysisMethodContext>();

builder.Services.AddScoped<ServerService>();
builder.Services.AddScoped<IServerContext, ServerContext>();

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ISessionContext, SessionContext>();

builder.Services.AddScoped<ResultsfileService>();
builder.Services.AddScoped<IResultsfileContext, ResultsfileContext>();

builder.Services.AddScoped<LapService>();
builder.Services.AddScoped<ILapContext, LapContext>();

builder.Services.AddScoped<LeaderboardlineService>();
builder.Services.AddScoped<ILeaderboardlineContext, LeaderboardlineContext>();

builder.Services.AddScoped<IncidentService>();
builder.Services.AddScoped<IIncidentContext, IncidentContext>();

builder.Services.AddScoped<IncidentEntryService>();
builder.Services.AddScoped<IIncidentEntryContext, IncidentEntryContext>();

builder.Services.AddScoped<SeriesDiscordchanneltypeService>();
builder.Services.AddScoped<ISeriesDiscordchanneltypeContext, SeriesDiscordchanneltypeContext>();

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
