using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using System.ComponentModel;
using CandyKeeper.Presentation.Views.Windows;

namespace CandyKeeper.Presentation
{
    public partial class MainWindow : Window
    {
        private bool IsMaximized = false;

        public MainWindow()
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
}