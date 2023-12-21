using GTRC_Database_API.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IBaseContext<ModelType> where ModelType : class, IBaseModel
    {
        public Task SaveChanges();
        public Task<List<ModelType>> GetAll();
        public Task<ModelType?> GetById(int id);
    }
}
