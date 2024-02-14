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
        UserDatetimeService UserDatetimeService,
        BopService BopService,
        BopTrackCarService BopTrackCarService,
        SeriesService SeriesService,
        SeasonService SeasonService,
        SeasonCarclassService SeasonCarclassService,
        OrganizationService OrganizationService,
        OrganizationUserService OrganizationUserService,
        TeamService TeamService,
        EntryService EntryService,
        EntryDatetimeService EntryDatetimeService,
        EventService EventService,
        EventCarclassService EventCarclassService,
        EventCarService EventCarService,
        EntryEventService EntryEventService,
        EntryUserEventService EntryUserEventService,
        PointssystemService PointssystemService,
        PointssystemPositionService PointssystemPositionService)
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
            Services[typeof(UserDatetime)] = UserDatetimeService;
            Services[typeof(Bop)] = BopService;
            Services[typeof(BopTrackCar)] = BopTrackCarService;
            Services[typeof(Series)] = SeriesService;
            Services[typeof(Season)] = SeasonService;
            Services[typeof(SeasonCarclass)] = SeasonCarclassService;
            Services[typeof(Organization)] = OrganizationService;
            Services[typeof(OrganizationUser)] = OrganizationUserService;
            Services[typeof(Team)] = TeamService;
            Services[typeof(Entry)] = EntryService;
            Services[typeof(EntryDatetime)] = EntryDatetimeService;
            Services[typeof(Event)] = EventService;
            Services[typeof(EventCarclass)] = EventCarclassService;
            Services[typeof(EventCar)] = EventCarService;
            Services[typeof(EntryEvent)] = EntryEventService;
            Services[typeof(EntryUserEvent)] = EntryUserEventService;
            Services[typeof(Pointssystem)] = PointssystemService;
            Services[typeof(PointssystemPosition)] = PointssystemPositionService;
        }

        public async Task ForceDelete(Type modelType, dynamic obj)
        {
            foreach (Type _modelType in GlobalValues.ModelTypeList)
            {
                var list = await Services[_modelType].GetChildObjects(modelType, obj.Id, false);
                foreach (var item in list) { await ForceDelete(_modelType, item); }
            }
            await Services[modelType].Delete(obj);
        }

        public async Task UpdateChildObjects(Type modelType, dynamic obj)
        {
            if (!Scripts.IsCompositeKey(modelType.Name))
            {
                foreach (Type _modelType in GlobalValues.ModelTypeList)
                {
                    var list = await Services[_modelType].GetChildObjects(modelType, obj.Id, false);
                    foreach (var item in list)
                    {
                        bool isValid = Services[_modelType].Validate(item);
                        bool isValidUniqProps = await Services[_modelType].ValidateUniqProps(item);
                        if (item is null || !isValidUniqProps) { await ForceDelete(_modelType, item); }
                        else if (!isValid)
                        {
                            await Services[_modelType].Update(item);
                            await UpdateChildObjects(_modelType, item);
                        }
                    }
                }
            }
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
