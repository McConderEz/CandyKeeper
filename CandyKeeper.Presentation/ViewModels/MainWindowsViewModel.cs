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
using System.Windows.Input;
using System.Windows.Markup;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class MainWindowsViewModel : ViewModel
    {
        public readonly ProductForSaleViewModel _productForSaleViewModel;
        private ViewModel _currentViewModel;

        public ViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(ref _currentViewModel, value);
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

        #endregion

        public MainWindowsViewModel(ProductForSaleViewModel productForSaleViewModel)
        {
            _productForSaleViewModel = productForSaleViewModel;           
           
            #region Команды
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecute);
            #endregion

        }
    }
}
