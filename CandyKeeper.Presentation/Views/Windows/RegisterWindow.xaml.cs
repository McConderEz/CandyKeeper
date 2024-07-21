using System.Windows;
using System.Windows.Input;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class RegisterWindow : Window
{
    private bool IsMaximized = false;
    
    public RegisterWindow()
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

    private void LoginBtn(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}