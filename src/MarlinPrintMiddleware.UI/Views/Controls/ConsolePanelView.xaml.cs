using System.Windows.Input;
using MarlinPrintMiddleware.UI.ViewModels;

namespace MarlinPrintMiddleware.UI.Views.Controls;

public partial class ConsolePanelView
{
    public ConsolePanelView()
    {
        InitializeComponent();
    }

    private void ConsoleInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || DataContext is not MainViewModel vm)
        {
            return;
        }

        if (vm.SendConsoleCommand.CanExecute(null))
        {
            vm.SendConsoleCommand.Execute(null);
            e.Handled = true;
        }
    }
}
