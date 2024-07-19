using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class SupplierWindow : UserControl
{
    
    public SupplierWindow()
    {
        InitializeComponent();
        SupplierViewModel.RefreshEvent += RefreshDataGridHandler;
        SupplierViewModel.CloseEvent += CloseView;
        SupplierViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        SuppliersDataGrid.Items.Refresh();
        if (DataContext is SupplierViewModel viewModel)
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