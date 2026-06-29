using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.UI.ViewModels;

namespace MarlinPrintMiddleware.UI.Views.Controls;

public partial class QueuePanelView
{
    private Point _dragStart;
    private PrintJob? _draggedJob;

    public QueuePanelView()
    {
        InitializeComponent();
        AllowDrop = true;
        Drop += OnDrop;
        DragOver += OnDragOver;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(PrintJob)))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            foreach (var file in files.Where(f =>
                         f.EndsWith(".gcode", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".gco", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".g", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    vm.IsBusy = true;
                    await vm.EnqueueFileAsync(file);
                }
                catch (Exception ex)
                {
                    vm.StatusMessage = ex.Message;
                }
                finally
                {
                    vm.IsBusy = false;
                }
            }

            return;
        }

        if (e.Data.GetData(typeof(PrintJob)) is PrintJob dragged
            && vm.SelectedJob is PrintJob target
            && dragged.Id != target.Id
            && dragged.Status == JobStatus.Pending
            && target.Status == JobStatus.Pending)
        {
            var newIndex = vm.Jobs.IndexOf(target);
            if (newIndex >= 0)
            {
                await vm.ReorderJobAsync(dragged.Id, newIndex);
            }
        }
    }

    private void JobsGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStart = e.GetPosition(null);
        _draggedJob = JobsGrid.SelectedItem as PrintJob;
    }

    private void JobsGrid_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _draggedJob is null)
        {
            return;
        }

        var position = e.GetPosition(null);
        if (Math.Abs(position.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(position.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        DragDrop.DoDragDrop(JobsGrid, _draggedJob, DragDropEffects.Move);
    }
}
