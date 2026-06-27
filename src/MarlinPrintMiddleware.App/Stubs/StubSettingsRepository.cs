using System.Collections.Concurrent;
using MarlinPrintMiddleware.Core.Interfaces;

namespace MarlinPrintMiddleware.App;

internal sealed class StubSettingsRepository : ISettingsRepository
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(key, out var value) && value is T typed)
        {
            return Task.FromResult<T?>(typed);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        _store[key] = value!;
        return Task.CompletedTask;
    }
}
