using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.Services
{
    public class FullService<ModelType> where ModelType : class, IBaseModel, new()
    {
        public readonly Dictionary<Type, dynamic> Services = [];

        public FullService(
        ColorService ColorService,
        SimService SimService,
        UserService UserService,
        TrackService TrackService,
        CarclassService CarclassService,
        ManufacturerService ManufacturerService,
        CarService CarService,
        RoleService RoleService,
        UserRoleService UserRoleService,
        BopService BopService,
        BopTrackCarService BopTrackCarService,
        SeriesService SeriesService,
        SeasonService SeasonService,
        SeasonCarclassService SeasonCarclassService,
        OrganizationService OrganizationService,
        OrganizationUserService OrganizationUserService,
        TeamService TeamService,
        EntryService EntryService,
        EventService EventService,
        EventCarclassService EventCarclassService,
        EventCarService EventCarService)
        {
            Services[typeof(Color)] = ColorService;
            Services[typeof(Sim)] = SimService;
            Services[typeof(User)] = UserService;
            Services[typeof(Track)] = TrackService;
            Services[typeof(Carclass)] = CarclassService;
            Services[typeof(Manufacturer)] = ManufacturerService;
            Services[typeof(Car)] = CarService;
            Services[typeof(Role)] = RoleService;
            Services[typeof(UserRole)] = UserRoleService;
            Services[typeof(Bop)] = BopService;
            Services[typeof(BopTrackCar)] = BopTrackCarService;
            Services[typeof(Series)] = SeriesService;
            Services[typeof(Season)] = SeasonService;
            Services[typeof(SeasonCarclass)] = SeasonCarclassService;
            Services[typeof(Organization)] = OrganizationService;
            Services[typeof(OrganizationUser)] = OrganizationUserService;
            Services[typeof(Team)] = TeamService;
            Services[typeof(Entry)] = EntryService;
            Services[typeof(Event)] = EventService;
            Services[typeof(EventCarclass)] = EventCarclassService;
            Services[typeof(EventCar)] = EventCarService;
        }

        public async Task<bool> HasChildObjects(int id, bool ignoreCompositeKeys)
        {
            foreach (Type modelType in GlobalValues.ModelTypeList)
            {
                if ((await Services[modelType].GetChildObjects(typeof(ModelType), id, ignoreCompositeKeys)).Count > 0) { return true; }
            }
            return false;
        }
    }
}
