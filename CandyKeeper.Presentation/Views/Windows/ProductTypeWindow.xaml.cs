using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class ProductTypeWindow : UserControl
{
    public ProductTypeWindow()
    {
        InitializeComponent();
        ProductTypeViewModel.RefreshEvent += RefreshDataGridHandler;
        ProductTypeViewModel.CloseEvent += CloseView;
        ProductTypeViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        ProductTypesDataGrid.Items.Refresh();
        if (DataContext is ProductTypeViewModel viewModel)
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