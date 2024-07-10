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
    internal class ProductDeliveryViewModel: ViewModel
    {
        private readonly IProductDeliveryService _service;

        private ObservableCollection<ProductDelivery> _productDeliveries;

        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            ProductDeliveries = new ObservableCollection<ProductDelivery>(await _service.Get());
        }

        #endregion
        
        public ObservableCollection<ProductDelivery> ProductDeliveries
        {
            get => _productDeliveries;
            set => Set(ref _productDeliveries, value);
        }

        public ProductDeliveryViewModel(IProductDeliveryService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _productDeliveries = new ObservableCollection<ProductDelivery>();
            OnGetCommandExecuted(null);
        }
    }
}
