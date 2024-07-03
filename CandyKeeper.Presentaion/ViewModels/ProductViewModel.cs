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
    internal class ProductViewModel : ViewModel
    {
        private readonly IProductService _service;

        private ObservableCollection<Product> _products;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => Set(ref _products, value);
        }

        public ProductViewModel(IProductService service)
        {
            _service = service;
        }
    }
}
