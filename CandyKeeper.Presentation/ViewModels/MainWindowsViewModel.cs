using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.Views.Windows;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class MainWindowsViewModel : ViewModel
    {
        private UserControl _currentView;
        private ViewModelLocator _viewModelLocator;

        public UserControl CurrentView
        {
            get => _currentView;
            set => Set(ref _currentView, value);
        }

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private void OnCloseApplicationCommandExecute(object p)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanCloseApplicationCommandExecute(object p) => true;
        #endregion

        #region SelectViewModelCommand
        public ICommand SelectViewCommand { get; }
        private void OnSelectViewCommandExecute(object p)
        {
            if (p is string viewmodel)
            {
                CurrentView = SelectViewModel(viewmodel);
            }
            else
            {
                throw new ArgumentNullException("p is null!");
            }
        }
        private bool CanSelectViewCommandExecute(object p) => true;
        #endregion
        
        #endregion

        public MainWindowsViewModel()
        {
            _viewModelLocator = new ViewModelLocator();
            
            #region Команды
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecute);
            SelectViewCommand = new LambdaCommand(OnSelectViewCommandExecute);
            #endregion
        }

        public UserControl SelectViewModel(string viewModelName) => viewModelName switch
        {
            "CityView" =>  new CityWindow(),
            "DistrictView" =>  new DistrictWindow(),
            "OwnershipTypeView" =>  new OwnershipTypeWindow(),
            "PackagingView" =>  new PackagingWindow(),
            "ProductDeliveryView" => new ProductDeliveryWindow(),
            "ProductForSaleView" => new ProductForSaleWindow(),
            "ProductTypeView" => new ProductTypeWindow(),
            "ProductView" => new ProductWindow(),
            "StoreView" => new StoreWindow(),
            "SupplierView" => new SupplierWindow(),
            _ => throw new ArgumentException("selected view model does not exist exist")
        };

    }
}
