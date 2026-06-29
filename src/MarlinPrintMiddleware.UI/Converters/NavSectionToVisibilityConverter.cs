using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MarlinPrintMiddleware.UI.Enums;

namespace MarlinPrintMiddleware.UI.Converters;

public sealed class NavSectionToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NavSection current || parameter is not string target
            || !Enum.TryParse<NavSection>(target, out var section))
        {
            return Visibility.Collapsed;
        }

        return current == section ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
