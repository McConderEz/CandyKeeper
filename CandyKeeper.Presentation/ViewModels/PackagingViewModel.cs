using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
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
    internal class PackagingViewModel: ViewModel
    {
        private readonly IPackagingService _service;

        private ObservableCollection<Packaging> _packagings;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            Packagings = new ObservableCollection<Packaging>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<Packaging> Packagings
        {
            get => _packagings;
            set => Set(ref _packagings, value);
        }

        public PackagingViewModel(IPackagingService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _packagings = new ObservableCollection<Packaging>();
            OnGetCommandExecuted(null);
        }
    }
}
