using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ColorService(IColorContext iColorContext,
        IBaseContext<Color> iBaseContext) : BaseService<Color>(iBaseContext)
    {
        public Color? Validate(Color? obj)
        {
            if (obj is null) { return null; }
            return obj;
        }

        public async Task<Color?> SetNextAvailable(Color? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            if (obj.Alpha == Byte.MinValue) { obj.Alpha = byte.MaxValue; } else { obj.Alpha--; }
            if (obj.Red == Byte.MinValue) { obj.Red = byte.MaxValue; } else { obj.Red--; }
            if (obj.Green == Byte.MinValue) { obj.Green = byte.MaxValue; } else { obj.Green--; }
            if (obj.Blue == Byte.MinValue) { obj.Blue = byte.MaxValue; } else { obj.Blue--; }
            for (byte alpha = byte.MinValue; alpha <= byte.MaxValue; alpha++)
            {
                if (obj.Alpha == Byte.MaxValue) { obj.Alpha = byte.MinValue; } else { obj.Alpha++; }
                for (byte red = byte.MinValue; red <= byte.MaxValue; red++)
                {
                    if (obj.Red == Byte.MaxValue) { obj.Red = byte.MinValue; } else { obj.Red++; }
                    for (byte green = byte.MinValue; green <= byte.MaxValue; green++)
                    {
                        if (obj.Green == Byte.MaxValue) { obj.Green = byte.MinValue; } else { obj.Green++; }
                        for (byte blue = byte.MinValue; blue <= byte.MaxValue; blue++)
                        {
                            if (obj.Blue == Byte.MaxValue) { obj.Blue = byte.MinValue; } else { obj.Blue++; }
                            if (await IsUnique(obj)) { return obj; }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Color?> GetTemp() { return await SetNextAvailable(new Color()); }
    }
}
