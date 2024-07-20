using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsProductTypePage : UserControl
{
    public DetailsProductTypePage()
    {
        InitializeComponent();
        ProductTypeViewModel.ReturnEvent += CloseView;
    }

    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}