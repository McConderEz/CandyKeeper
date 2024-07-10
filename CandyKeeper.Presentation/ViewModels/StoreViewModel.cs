using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CandyKeeper.Presentation.Infrastructure.Commands;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class StoreViewModel : ViewModel
    {
        private readonly IStoreService _service;

        private ObservableCollection<Store> _stores;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            Stores = new ObservableCollection<Store>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => Set(ref _stores, value);
        }

        public StoreViewModel(IStoreService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _stores = new ObservableCollection<Store>();
            OnGetCommandExecuted(null);
        }
    }
}
