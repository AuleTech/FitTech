using Blazored.LocalStorage;
using FitTech.WebComponents.Persistence;

namespace FitTech.Trainer.Wasm.Persistence;

internal sealed class BlazorLocalStorage : IStorage
{
    private readonly ILocalStorageService _localStorage;

    public BlazorLocalStorage(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        await _localStorage.ClearAsync(cancellationToken);
    }

    public async Task SetItemAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        await _localStorage.SetItemAsync<T>(key, value, cancellationToken);
    }

    public async Task<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        return await _localStorage.GetItemAsync<T>(key, cancellationToken);
    }

    public async Task<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken)
    {
        return await _localStorage.ContainKeyAsync(key, cancellationToken);
    }
}
