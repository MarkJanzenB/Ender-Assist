using System.Windows;
using MarlinPrintMiddleware.UI.ViewModels;

namespace MarlinPrintMiddleware.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
