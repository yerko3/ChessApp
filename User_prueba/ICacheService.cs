namespace User_prueba
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task RemoveDataAsync(string key); // Nuevo método
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    }
}