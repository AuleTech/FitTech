namespace FitTech.WebComponents.Persistence;

public interface IStorage
{
    Task ClearAsync(CancellationToken cancellationToken);
    Task SetItemAsync<T>(string key, T value, CancellationToken cancellationToken);
    Task<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken) where T : class;
    Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken);

}
