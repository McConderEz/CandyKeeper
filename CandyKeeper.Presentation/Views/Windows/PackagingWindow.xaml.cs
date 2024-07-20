using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class PackagingWindow : UserControl
{
    public PackagingWindow()
    {
        InitializeComponent();
        PackagingViewModel.RefreshEvent += RefreshDataGridHandler;
        PackagingViewModel.CloseEvent += CloseView;
        PackagingViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        PackagingsDataGrid.Items.Refresh();
        if (DataContext is PackagingViewModel viewModel)
        {
            viewModel.OnGetCommandExecuted(null);
        }
    }

    public void CloseView(object p)
    {
        this.DataGridView.Visibility = Visibility.Collapsed;
    }

    public void OpenView(object p)
    {
        this.DataGridView.Visibility = Visibility.Visible;
    }
}