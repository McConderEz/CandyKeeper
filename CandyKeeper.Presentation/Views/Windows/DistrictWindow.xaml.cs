using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class DistrictWindow : UserControl
{
    public DistrictWindow()
    {
        InitializeComponent();
        DistrictViewModel.RefreshEvent += RefreshDataGridHandler;
        DistrictViewModel.CloseEvent += CloseView;
        DistrictViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        DistrictDataGrid.Items.Refresh();
        if (DataContext is DistrictViewModel viewModel)
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