using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;

namespace CandyKeeper.Presentation.Views.AddEditPages;

public partial class AddEditSupplierPage : Window
{
    private readonly IStoreService _storeService;
    
    private bool IsMaximized = false;
    
    
    public AddEditSupplierPage()
    {
        InitializeComponent();
    }


    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
        {
            if (IsMaximized)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1080;
                this.Height = 720;

                IsMaximized = false;
            }
            else
            {
                this.WindowState = WindowState.Normal;

                IsMaximized = true;
            }
        }
    }

    private void CloseBtn(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
}