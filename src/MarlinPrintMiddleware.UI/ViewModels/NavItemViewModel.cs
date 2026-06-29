using CommunityToolkit.Mvvm.ComponentModel;
using MarlinPrintMiddleware.UI.Enums;

namespace MarlinPrintMiddleware.UI.ViewModels;

public partial class NavItemViewModel : ObservableObject
{
    public NavItemViewModel(NavSection section, string label, string iconData)
    {
        Section = section;
        Label = label;
        IconData = iconData;
    }

    public NavSection Section { get; }

    public string Label { get; }

    public string IconData { get; }

    [ObservableProperty] private bool _isSelected;
}
