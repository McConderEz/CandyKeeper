using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsProductDeliveryPage : UserControl
{
    public DetailsProductDeliveryPage()
    {
        InitializeComponent();
        ProductDeliveryViewModel.ReturnEvent += CloseView;
    }

    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}