using System.Text.RegularExpressions;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Serial.Parsers;

/// <summary>
/// Parses Marlin M115 handshake responses into <see cref="FirmwareInfo"/>.
/// </summary>
public sealed class MarlinHandshakeParser
{
    private static readonly Regex FirmwareVersionRegex = new(
        @"FIRMWARE_VERSION:([^\s\r\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex FirmwareNameRegex = new(
        @"FIRMWARE_NAME:([^\r\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex MarlinVersionRegex = new(
        @"Marlin\s+([\d.]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex MachineTypeRegex = new(
        @"MACHINE_TYPE:([^\r\n]+?)(?:\s+(?:EXTRUDER_COUNT|UUID|Cap:)|\r|\n|$)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public FirmwareInfo? Parse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return null;
        }

        var isMarlin = response.Contains("Marlin", StringComparison.OrdinalIgnoreCase);

        if (!isMarlin)
        {
            return new FirmwareInfo { IsMarlin = false };
        }

        var info = new FirmwareInfo { IsMarlin = true };

        var firmwareNameMatch = FirmwareNameRegex.Match(response);
        if (firmwareNameMatch.Success)
        {
            var firmwareNameValue = firmwareNameMatch.Groups[1].Value.Trim();
            var marlinVersionMatch = MarlinVersionRegex.Match(firmwareNameValue);
            if (marlinVersionMatch.Success)
            {
                info.FirmwareName = "Marlin";
                info.Version = marlinVersionMatch.Groups[1].Value;
            }
            else
            {
                info.FirmwareName = firmwareNameValue.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }
        else
        {
            info.FirmwareName = "Marlin";
        }

        var firmwareVersionMatch = FirmwareVersionRegex.Match(response);
        if (firmwareVersionMatch.Success)
        {
            info.Version = firmwareVersionMatch.Groups[1].Value;
        }
        else if (string.IsNullOrEmpty(info.Version))
        {
            var fallbackVersionMatch = MarlinVersionRegex.Match(response);
            if (fallbackVersionMatch.Success)
            {
                info.Version = fallbackVersionMatch.Groups[1].Value;
            }
        }

        var machineTypeMatch = MachineTypeRegex.Match(response);
        if (machineTypeMatch.Success)
        {
            info.MachineType = machineTypeMatch.Groups[1].Value.Trim();
        }

        return info;
    }

    public FirmwareInfo? Parse(IEnumerable<string> lines) =>
        Parse(string.Join('\n', lines));
}
