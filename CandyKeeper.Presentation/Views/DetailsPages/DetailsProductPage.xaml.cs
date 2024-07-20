using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsProductPage : UserControl
{
    public DetailsProductPage()
    {
        InitializeComponent();
        ProductViewModel.ReturnEvent += CloseView;
    }

    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}