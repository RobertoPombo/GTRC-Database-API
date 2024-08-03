using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ServerService(IServerContext iServerContext,
        IBaseContext<Sim> iSimContext,
        IBaseContext<Server> iBaseContext) : BaseService<Server>(iBaseContext)
    {
        public bool Validate(Server? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.PortUdp > Server.MaxPort) { obj.PortUdp = Server.MaxPort; isValid = false; }
            if (obj.PortTcp > Server.MaxPort) { obj.PortTcp = Server.MaxPort; isValid = false; }
            Sim? sim = null;
            if (obj.Sim is not null) { sim = iSimContext.GetById(obj.SimId).Result; };
            if (sim is null)
            {
                List<Sim> list = iSimContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Sim = list[0]; obj.SimId = list[0].Id; isValid = false; }
            }
            else { obj.Sim = sim; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Server? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            ushort startValuePortUdp = obj.PortUdp;
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                if (obj.PortUdp == Server.MaxPort) { obj.PortUdp = ushort.MinValue; } else { obj.PortUdp++; }
                if (obj.PortUdp == startValuePortUdp) { obj = null; return false; }
            }

            ushort startValuePortTcp = obj.PortTcp;
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                if (obj.PortTcp == Server.MaxPort) { obj.PortTcp = ushort.MinValue; } else { obj.PortTcp++; }
                if (obj.PortTcp == startValuePortTcp) { obj = null; return false; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Server?> GetTemp() { Server obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
