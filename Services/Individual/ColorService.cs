using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ColorService(IColorContext iColorContext,
        IBaseContext<Color> iBaseContext) : BaseService<Color>(iBaseContext)
    {
        public bool Validate(Color? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Color? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            if (obj.Alpha == byte.MinValue) { obj.Alpha = byte.MaxValue; } else { obj.Alpha--; }
            if (obj.Red == byte.MinValue) { obj.Red = byte.MaxValue; } else { obj.Red--; }
            if (obj.Green == byte.MinValue) { obj.Green = byte.MaxValue; } else { obj.Green--; }
            if (obj.Blue == byte.MinValue) { obj.Blue = byte.MaxValue; } else { obj.Blue--; }
            for (byte alpha = byte.MinValue; alpha <= byte.MaxValue; alpha++)
            {
                if (obj.Alpha == byte.MaxValue) { obj.Alpha = byte.MinValue; } else { obj.Alpha++; }
                for (byte red = byte.MinValue; red <= byte.MaxValue; red++)
                {
                    if (obj.Red == byte.MaxValue) { obj.Red = byte.MinValue; } else { obj.Red++; }
                    for (byte green = byte.MinValue; green <= byte.MaxValue; green++)
                    {
                        if (obj.Green == byte.MaxValue) { obj.Green = byte.MinValue; } else { obj.Green++; }
                        for (byte blue = byte.MinValue; blue <= byte.MaxValue; blue++)
                        {
                            if (obj.Blue == byte.MaxValue) { obj.Blue = byte.MinValue; } else { obj.Blue++; }
                            if (await IsUnique(obj)) { Validate(obj); return isValidUniqProps; } else { isValidUniqProps = false; }
                        }
                    }
                }
            }

            obj = null;
            return false;
        }

        public async Task<Color?> GetTemp() { Color obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
