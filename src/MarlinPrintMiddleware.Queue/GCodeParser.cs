using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Queue;

public sealed class GCodeParser
{
    private static readonly string[] AllowedExtensions = [".gcode", ".gco", ".g"];

    public void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new PrintQueueException("G-code file path is required.");
        }

        if (!File.Exists(filePath))
        {
            throw new PrintQueueException($"G-code file not found: {filePath}");
        }

        var extension = Path.GetExtension(filePath);
        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new PrintQueueException($"Unsupported file extension: {extension}");
        }
    }

    public async Task<GCodeFileInfo> AnalyzeAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath);

        var totalLines = 0;
        var printableLines = 0;
        string? slicerName = null;
        TimeSpan? estimatedDuration = null;

        await foreach (var line in ReadRawLinesAsync(filePath, cancellationToken).ConfigureAwait(false))
        {
            totalLines++;

            if (totalLines <= 50)
            {
                slicerName ??= TryParseSlicer(line);
                estimatedDuration ??= TryParseEstimatedDuration(line);
            }

            if (!IsSkippableLine(line))
            {
                printableLines++;
            }
        }

        return new GCodeFileInfo
        {
            FilePath = filePath,
            Name = Path.GetFileName(filePath),
            TotalLines = totalLines,
            PrintableLines = printableLines,
            SlicerName = slicerName,
            EstimatedDuration = estimatedDuration
        };
    }

    public async IAsyncEnumerable<GCodeLine> ReadLinesAsync(
        string filePath,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath);

        var lineNumber = 0;
        await foreach (var raw in ReadRawLinesAsync(filePath, cancellationToken).ConfigureAwait(false))
        {
            lineNumber++;
            var trimmed = raw.TrimEnd('\r', '\n');
            var isComment = trimmed.Length == 0 || trimmed.StartsWith(';');

            yield return new GCodeLine
            {
                LineNumber = lineNumber,
                Content = isComment ? trimmed : trimmed.Trim(),
                IsComment = isComment
            };
        }
    }

    private static async IAsyncEnumerable<string> ReadRawLinesAsync(
        string filePath,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is not null)
            {
                yield return line;
            }
        }
    }

    private static bool IsSkippableLine(string line)
    {
        var trimmed = line.Trim();
        return trimmed.Length == 0 || trimmed.StartsWith(';');
    }

    private static string? TryParseSlicer(string line)
    {
        if (line.Contains("Cura", StringComparison.OrdinalIgnoreCase))
        {
            return "Cura";
        }

        if (line.Contains("PrusaSlicer", StringComparison.OrdinalIgnoreCase))
        {
            return "PrusaSlicer";
        }

        return null;
    }

    private static TimeSpan? TryParseEstimatedDuration(string line)
    {
        const string prefix = ";TIME:";
        if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (int.TryParse(line[prefix.Length..].Trim(), out var seconds))
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return null;
    }
}
