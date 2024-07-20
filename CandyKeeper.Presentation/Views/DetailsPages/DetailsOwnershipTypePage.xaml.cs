﻿using System.Windows;
using System.Windows.Controls;
using CandyKeeper.Presentation.ViewModels;

namespace CandyKeeper.Presentation.Views.DetailsPages;

public partial class DetailsOwnershipTypePage : UserControl
{
    public DetailsOwnershipTypePage()
    {
        InitializeComponent();
        OwnershipTypeViewModel.ReturnEvent += CloseView;
    }
    
    public void CloseView(object p)
    {
        this.Visibility = Visibility.Collapsed;
    }
}