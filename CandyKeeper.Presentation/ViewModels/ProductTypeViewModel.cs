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
using System.Windows.Input;
using CandyKeeper.Presentation.Infrastructure.Commands;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductTypeViewModel: ViewModel
    {
        private readonly IProductTypeService _service;

        private ObservableCollection<ProductType> _productTypes;
        
        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            ProductTypes = new ObservableCollection<ProductType>(await _service.Get());
        }

        #endregion
        

        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set => Set(ref _productTypes, value);
        }

        public ProductTypeViewModel(IProductTypeService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _productTypes = new ObservableCollection<ProductType>();
            OnGetCommandExecuted(null);
        }
    }
}
