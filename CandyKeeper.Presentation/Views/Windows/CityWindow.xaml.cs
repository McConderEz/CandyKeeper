﻿using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.Windows;

public partial class CityWindow : UserControl
{
    public CityWindow()
    {
        InitializeComponent();
        CityViewModel.RefreshEvent += RefreshDataGridHandler;
        CityViewModel.CloseEvent += CloseView;
        CityViewModel.ReturnEvent += OpenView;
    }
    
    public void RefreshDataGridHandler(object p)
    {
        CityDataGrid.Items.Refresh();
        if (DataContext is CityViewModel viewModel)
        {
            viewModel.OnGetCommandExecuted(p);
        }
    }

    public void CloseView(object p)
    {
        this.DataGridView.Visibility = Visibility.Collapsed;
    }

    public void OpenView(object p)
    {
        this.DataGridView.Visibility = Visibility.Visible;
    }
}