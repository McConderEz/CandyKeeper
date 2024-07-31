using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels;
using CandyKeeper.Presentation.Views.AddEditPages;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class ProductForSaleWindow : UserControl
{
    
    public ProductForSaleWindow()
    {
        InitializeComponent();
        ProductForSaleViewModel.RefreshEvent += RefreshDataGridHandler;
        ProductForSaleViewModel.CloseEvent += CloseView;
        ProductForSaleViewModel.ReturnEvent += OpenView;
    }

    public void RefreshDataGridHandler(object p)
    {
        ProductForSaleDataGrid.Items.Refresh();
        if (DataContext is ProductForSaleViewModel viewModel)
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