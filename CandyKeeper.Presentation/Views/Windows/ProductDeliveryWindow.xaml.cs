using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class ProductDeliveryWindow : UserControl
{
    public ProductDeliveryWindow()
    {
        InitializeComponent();
        ProductDeliveryViewModel.RefreshEvent += RefreshDataGridHandler;
        ProductDeliveryViewModel.CloseEvent += CloseView;
        ProductDeliveryViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        ProductDeliveryDataGrid.Items.Refresh();
        if (DataContext is ProductDeliveryViewModel viewModel)
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