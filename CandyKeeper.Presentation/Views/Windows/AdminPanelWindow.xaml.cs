using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class AdminPanelWindow : UserControl
{
    public AdminPanelWindow()
    {
        InitializeComponent();
    }
    
    public void RefreshDataGridHandler(object p)
    {
        CityDataGrid.Items.Refresh();
        if (DataContext is ProductViewModel viewModel)
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