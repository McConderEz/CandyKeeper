using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsProductForSalePage : UserControl
{
    public DetailsProductForSalePage()
    {
        InitializeComponent();
        ProductForSaleViewModel.ReturnEvent += CloseView;
    }

    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}