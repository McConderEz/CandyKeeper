using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using DynamicData.Binding;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductForSaleViewModel : ViewModel
    {
        private readonly IProductForSaleService _service;
        private ObservableCollection<ProductForSale> _productForSales;

        public ObservableCollection<ProductForSale> ProductForSales
        {
            get => _productForSales;
            set => Set(ref _productForSales, value);
        }

        public ProductForSaleViewModel(IProductForSaleService service)
        {
            _service = service;

            _productForSales = new ObservableCollection<ProductForSale>(_service.Get().Result);
        }

    }
}
