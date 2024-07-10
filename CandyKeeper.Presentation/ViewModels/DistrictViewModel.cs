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
    internal class DistrictViewModel : ViewModel
    {
        private readonly IDistrictService _service;

        private ObservableCollection<District> _districts;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            Districts = new ObservableCollection<District>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<District> Districts
        {
            get => _districts;
            set => Set(ref _districts, value);
        }

        public DistrictViewModel(IDistrictService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _districts = new ObservableCollection<District>();
            OnGetCommandExecuted(null);
        }
    }
}
