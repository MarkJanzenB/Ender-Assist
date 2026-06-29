using System.Text.Json;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Persistence.Repositories;

public sealed class MacroRepository : IMacroRepository
{
    public const string SettingsKey = "printer.macros";

    private readonly ISettingsRepository _settingsRepository;

    public MacroRepository(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<IReadOnlyList<PrinterMacro>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var macros = await _settingsRepository
            .GetAsync<List<PrinterMacro>>(SettingsKey, cancellationToken)
            .ConfigureAwait(false);

        return macros ?? [];
    }

    public Task SaveAsync(IReadOnlyList<PrinterMacro> macros, CancellationToken cancellationToken = default) =>
        _settingsRepository.SetAsync(SettingsKey, macros.ToList(), cancellationToken);
}
