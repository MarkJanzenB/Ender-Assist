using System.Text.RegularExpressions;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Monitoring.Parsers;

public static partial class MarlinTemperatureParser
{
    [GeneratedRegex(@"T:([\d.]+)(?:\s*/([\d.]+))?", RegexOptions.IgnoreCase)]
    private static partial Regex HotendRegex();

    [GeneratedRegex(@"B:([\d.]+)(?:\s*/([\d.]+))?", RegexOptions.IgnoreCase)]
    private static partial Regex BedRegex();

    public static bool TryParse(string response, out double hotend, out double targetHotend, out double bed, out double targetBed)
    {
        hotend = targetHotend = bed = targetBed = 0;

        var hotendMatch = HotendRegex().Match(response);
        if (!hotendMatch.Success || !double.TryParse(hotendMatch.Groups[1].Value, out hotend))
        {
            return false;
        }

        if (hotendMatch.Groups[2].Success)
        {
            double.TryParse(hotendMatch.Groups[2].Value, out targetHotend);
        }

        var bedMatch = BedRegex().Match(response);
        if (bedMatch.Success && double.TryParse(bedMatch.Groups[1].Value, out bed))
        {
            if (bedMatch.Groups[2].Success)
            {
                double.TryParse(bedMatch.Groups[2].Value, out targetBed);
            }
        }

        return true;
    }
}

public static partial class MarlinPositionParser
{
    [GeneratedRegex(@"X:([\d.-]+)\s+Y:([\d.-]+)\s+Z:([\d.-]+)", RegexOptions.IgnoreCase)]
    private static partial Regex PositionRegex();

    public static bool TryParse(string response, out double x, out double y, out double z)
    {
        x = y = z = 0;
        var match = PositionRegex().Match(response);
        if (!match.Success)
        {
            return false;
        }

        return double.TryParse(match.Groups[1].Value, out x)
            && double.TryParse(match.Groups[2].Value, out y)
            && double.TryParse(match.Groups[3].Value, out z);
    }
}
