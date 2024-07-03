using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class CityViewModel : ViewModel
    {
        private readonly ICityService _service;

        private ObservableCollection<City> _cities;

        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
        }

        public CityViewModel(ICityService service)
        {
            _service = service;
        }
    }
}
