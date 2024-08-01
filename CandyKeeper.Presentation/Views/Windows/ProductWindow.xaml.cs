using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class ProductWindow : UserControl
{
    public ProductWindow()
    {
        InitializeComponent();
        ProductViewModel.RefreshEvent += RefreshDataGridHandler;
        ProductViewModel.CloseEvent += CloseView;
        ProductViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        ProductDataGrid.Items.Refresh();
        if (DataContext is ProductViewModel viewModel)
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