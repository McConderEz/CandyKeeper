using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class OwnershipTypeWindow : UserControl
{
    public OwnershipTypeWindow()
    {
        InitializeComponent();
        OwnershipTypeViewModel.RefreshEvent += RefreshDataGridHandler;
        OwnershipTypeViewModel.CloseEvent += CloseView;
        OwnershipTypeViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        OwnershipTypeDataGrid.Items.Refresh();
        if (DataContext is OwnershipTypeViewModel viewModel)
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