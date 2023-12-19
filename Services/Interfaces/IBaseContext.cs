namespace GTRC_Database_API.Services.Interfaces
{
    public interface IBaseContext<ModelType>
    {
        public Task SaveChanges();
        public Task<List<ModelType>> GetAll();
    }
}
