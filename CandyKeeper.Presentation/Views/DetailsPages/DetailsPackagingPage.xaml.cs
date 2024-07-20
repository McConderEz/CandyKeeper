using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsPackagingPage : UserControl
{
    public DetailsPackagingPage()
    {
        InitializeComponent();
        PackagingViewModel.ReturnEvent += CloseView;
    }

    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}