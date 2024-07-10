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
    internal class OwnershipTypeViewModel: ViewModel
    {
        private readonly IOwnershipTypeService _service;

        private ObservableCollection<OwnershipType> _ownershipTypes;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            OwnershipTypes = new ObservableCollection<OwnershipType>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<OwnershipType> OwnershipTypes
        {
            get => _ownershipTypes;
            set => Set(ref _ownershipTypes, value);
        }

        public OwnershipTypeViewModel(IOwnershipTypeService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _ownershipTypes = new ObservableCollection<OwnershipType>();
            OnGetCommandExecuted(null);
        }
    }
}
