using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;
using CandyKeeper.Presentation.ViewModels.Base;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class AdminPanelWindow : UserControl
{
    public AdminPanelWindow()
    {
        InitializeComponent();
        UserViewModel.RefreshEvent += RefreshDataGridHandler;
    }

    private void RefreshDataGridHandler(object? sender, EventArgs e)
    {
        UsersDataGrid.Items.Refresh();
        if (DataContext is UserViewModel viewModel)
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