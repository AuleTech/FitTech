using FitTech.WebComponents.Persistence;
using Newtonsoft.Json;

namespace FitTech.Client.Mobile.Persistence;

internal sealed class MauiStorage : IStorage
{
    private readonly IPreferences _preferences;

    public MauiStorage(IPreferences preferences)
    {
        _preferences = preferences;
    }

    public Task ClearAsync(CancellationToken cancellationToken)
    {
        _preferences.Clear();
        return Task.CompletedTask;
    }

    public Task SetItemAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        _preferences.Set(key, JsonConvert.SerializeObject(value));

        return Task.CompletedTask;
    }


    public Task<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        var value = _preferences.Get<string>(key, string.Empty);

        return Task.FromResult(string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<T>(value));
    }

    public Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken)
    {
        return Task.FromResult(_preferences.ContainsKey(key));
    }
}
