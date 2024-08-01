using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class StoreWindow : UserControl
{
    public StoreWindow()
    {
        InitializeComponent();
        StoreViewModel.RefreshEvent += RefreshDataGridHandler;
        StoreViewModel.CloseEvent += CloseView;
        StoreViewModel.ReturnEvent += OpenView;
    }

    public void RefreshDataGridHandler(object p)
    {
        StoreDataGrid.Items.Refresh();
        if (DataContext is StoreViewModel viewModel)
        {
            viewModel.OnGetCommandExecuted(p);
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