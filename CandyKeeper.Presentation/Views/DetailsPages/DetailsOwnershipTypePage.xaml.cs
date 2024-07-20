using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsOwnershipTypePage : UserControl
{
    public DetailsOwnershipTypePage()
    {
        InitializeComponent();
        OwnershipTypeViewModel.ReturnEvent += CloseView;
        OwnershipTypeViewModel.RefreshEvent += RefreshDataGridHandler;
    }

    
    public void RefreshDataGridHandler(object p)
    {
        StoresDataGrid.Items.Refresh();
        if (DataContext is OwnershipTypeViewModel viewModel)
        {
            viewModel.OnDetailsCommandExecuted(viewModel.SelectedItemForDetails.Id);
        }
    }
    
    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}