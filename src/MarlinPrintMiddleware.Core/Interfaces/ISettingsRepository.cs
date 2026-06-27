namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Typed key-value settings persistence.
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Retrieves a setting value by key.
    /// </summary>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <param name="key">Setting key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stored value, or default if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a setting value by key.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="key">Setting key.</param>
    /// <param name="value">Value to persist.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
}
