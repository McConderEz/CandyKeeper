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
    internal class ProductTypeViewModel: ViewModel
    {
        private readonly IProductTypeService _service;

        private ObservableCollection<ProductType> _productTypes;

        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set => Set(ref _productTypes, value);
        }

        public ProductTypeViewModel(IProductTypeService service)
        {
            _service = service;
        }
    }
}
