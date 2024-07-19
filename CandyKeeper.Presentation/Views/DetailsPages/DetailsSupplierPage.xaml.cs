using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsSupplierPage : UserControl
{
    public DetailsSupplierPage()
    {
        InitializeComponent();
        SupplierViewModel.ReturnEvent += CloseView;
        SupplierViewModel.RefreshEvent += RefreshDataGridHandler;
    }

    
    public void RefreshDataGridHandler(object p)
    {
        StoreDataGrid.Items.Refresh();
        if (DataContext is SupplierViewModel viewModel)
        {
            viewModel.OnDetailsCommandExecuted(viewModel.SelectedItemForDetails.Id);
        }
    }
    
    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}