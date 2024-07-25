using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using CandyKeeper.Presentation.Models;
using CandyKeeper.Presentation.ViewModels;
using CandyKeeper.Presentation.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class LoginWindow : Window
{
    private bool IsMaximized = false;
    
    public LoginWindow()
    {
        InitializeComponent();
        UserViewModel.CloseEvent += HideWindow;
        UserViewModel.ShowMainEvent += HideWindow;
        MainWindowsViewModel.LeaveAccountEvent += HideWindow;
    }

    private void HideWindow(object? sender, User e)
    {
        if (this.Visibility == Visibility.Visible)
        {
            this.Hide();
        }
        else
        {
            this.Show();
        }
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