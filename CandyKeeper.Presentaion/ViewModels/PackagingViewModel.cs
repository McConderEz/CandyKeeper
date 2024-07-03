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

namespace CandyKeeper.Presentation.ViewModels
{
    internal class PackagingViewModel: ViewModel
    {
        private readonly IPackagingService _service;

        private ObservableCollection<Packaging> _packagings;

        public ObservableCollection<Packaging> Packagings
        {
            get => _packagings;
            set => Set(ref _packagings, value);
        }

        public PackagingViewModel(IPackagingService service)
        {
            _service = service;
        }
    }
}
