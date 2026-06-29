using System.Windows;

namespace MarlinPrintMiddleware.UI.Themes;

public static class ThemeManager
{
  private static readonly Uri DarkUri = new(
      "pack://application:,,,/MarlinPrintMiddleware.UI;component/Themes/EnderAssistTheme.xaml",
      UriKind.Absolute);

  private static readonly Uri LightUri = new(
      "pack://application:,,,/MarlinPrintMiddleware.UI;component/Themes/EnderAssistTheme.Light.xaml",
      UriKind.Absolute);

  public static void Apply(bool isDark)
  {
    if (Application.Current is null)
    {
      return;
    }

    var theme = new ResourceDictionary { Source = isDark ? DarkUri : LightUri };

    if (Application.Current.Resources.MergedDictionaries.Count > 0)
    {
      Application.Current.Resources.MergedDictionaries[0] = theme;
    }
    else
    {
      Application.Current.Resources.MergedDictionaries.Add(theme);
    }
  }
}
