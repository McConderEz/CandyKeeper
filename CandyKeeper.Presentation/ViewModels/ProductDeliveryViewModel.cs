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
    internal class ProductDeliveryViewModel: ViewModel
    {
        private readonly IProductDeliveryService _service;

        private ObservableCollection<ProductDelivery> _productDeliveries;

        public ObservableCollection<ProductDelivery> ProductDeliveries
        {
            get => _productDeliveries;
            set => Set(ref _productDeliveries, value);
        }

        public ProductDeliveryViewModel(IProductDeliveryService service)
        {
            _service = service;
        }
    }
}
