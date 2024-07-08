using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class DistrictViewModel : ViewModel
    {
        private readonly IDistrictService _service;

        private ObservableCollection<District> _districts;

        public ObservableCollection<District> Districts
        {
            get => _districts;
            set => Set(ref _districts, value);
        }

        public DistrictViewModel(IDistrictService service)
        {
            _service = service;
        }
    }
}
