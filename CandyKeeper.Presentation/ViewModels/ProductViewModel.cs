using CandyKeeper.Application.Interfaces;
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
    internal class ProductViewModel : ViewModel
    {
        private readonly IProductService _service;

        private ObservableCollection<Product> _products;
        
        #region Команды
        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            Products = new ObservableCollection<Product>(await _service.Get());
        }

        #endregion

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => Set(ref _products, value);
        }

        public ProductViewModel(IProductService service)
        {
            _service = service;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            _products = new ObservableCollection<Product>();
            OnGetCommandExecuted(null);
        }


    }
}
