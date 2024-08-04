using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.EfcContext
{
    public class StintAnalysisMethodContext(DataContext db) : IStintAnalysisMethodContext
    {

    }
}
