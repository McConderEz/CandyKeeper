using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CandyKeeper.Presentation.Infrastructure.Commands;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class CityViewModel : ViewModel
    {
        private readonly ICityService _service;

        private ObservableCollection<City> _cities;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            Cities = new ObservableCollection<City>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
        }

        public CityViewModel(ICityService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _cities = new ObservableCollection<City>();
            OnGetCommandExecuted(null);
        }
    }
}
