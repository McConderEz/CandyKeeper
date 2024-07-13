using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsStorePage : UserControl
{
    public DetailsStorePage()
    {
        InitializeComponent();
        StoreViewModel.ReturnEvent += CloseView;
        StoreViewModel.RefreshEvent += RefreshDataGridHandler;
    }

    
    public void RefreshDataGridHandler(object p)
    {
        SupplierDataGrid.Items.Refresh();
        if (DataContext is StoreViewModel viewModel)
        {
            viewModel.OnDetailsCommandExecuted(viewModel.SelectedItemForDetails.Id);
        }
    }
    
    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}