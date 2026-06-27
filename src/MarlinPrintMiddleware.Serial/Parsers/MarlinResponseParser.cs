namespace MarlinPrintMiddleware.Serial.Parsers;

/// <summary>
/// Parses Marlin protocol response lines for ok, busy, and error states.
/// </summary>
public static class MarlinResponseParser
{
    public static bool IsOk(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        var trimmed = line.Trim();

        if (trimmed.Equals("ok", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (trimmed.EndsWith(" ok", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    public static bool IsBusy(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        var trimmed = line.Trim();
        return trimmed.StartsWith("busy", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsError(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        var trimmed = line.Trim();
        return trimmed.StartsWith("Error:", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("error:", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetErrorMessage(string line)
    {
        if (!IsError(line))
        {
            return string.Empty;
        }

        var trimmed = line.Trim();
        var colonIndex = trimmed.IndexOf(':');
        if (colonIndex < 0 || colonIndex >= trimmed.Length - 1)
        {
            return trimmed;
        }

        return trimmed[(colonIndex + 1)..].Trim();
    }
}
