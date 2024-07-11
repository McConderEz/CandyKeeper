using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CandyKeeper.Presentation.Views.AddEditPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductForSaleViewModel : ViewModel
    {
        private readonly IProductForSaleService _service;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly IProductDeliveryService _productDeliveryService;
        private readonly IPackagingService _packagingService;
        
        private ObservableCollection<ProductForSale> _productForSales;
        
        private ObservableCollection<Product> _products;
        private ObservableCollection<Store> _stores;
        private ObservableCollection<ProductDelivery> _productDeliveries;
        private ObservableCollection<Packaging> _packagings;

        private Models.ProductForSale _selectedItem = new();

        public Models.ProductForSale SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        #region Команды

        #region  GetCommand

        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            ProductForSales = new ObservableCollection<ProductForSale>(await _service.Get());
        }
        
        #endregion

        public ICommand AddEditShowCommand { get; }
        private bool CanAddEditShowCommandExecute(object p) => true;
        public async void OnAddEditShowCommandExecuted(object p)
        {
            if (p is int id)
            {
                SelectedItem.Id = id;
            }
            
            await LoadComboBoxes();
            var page = new AddEditProductForSalePage();
            page.DataContext = this;
            page.Show();
        }
        
        public ICommand AddEditCommand { get; }
        private bool CanAddEditCommandExecute(object p) => true;
        public async void OnAddEditCommandExecuted(object p)
        {
            if (SelectedItem.Id == 0)
            {
                var productForSale = ProductForSale.Create(
                    _selectedItem.Id,
                    _selectedItem.ProductId,
                    _selectedItem.StoreId,
                    _selectedItem.ProductDeliveryId,
                    _selectedItem.PackagingId,
                    _selectedItem.Price,
                    _selectedItem.Volume).Value;
                await _service.Create(productForSale);
            }
            else
            {
                var productForSale = ProductForSale.Create(
                    _selectedItem.Id,
                    _selectedItem.ProductId,
                    _selectedItem.StoreId,
                    _selectedItem.ProductDeliveryId,
                    _selectedItem.PackagingId,
                    _selectedItem.Price,
                    _selectedItem.Volume).Value;
                await _service.Update(productForSale);
            }
        }
        
        #endregion

        public ObservableCollection<ProductForSale> ProductForSales
        {
            get => _productForSales;
            set => Set(ref _productForSales, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => Set(ref _products, value);
        }
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => Set(ref _stores, value);
        }
        public ObservableCollection<ProductDelivery> ProductDeliveries
        {
            get => _productDeliveries;
            set => Set(ref _productDeliveries, value);
        }
        public ObservableCollection<Packaging> Packagings
        {
            get => _packagings;
            set => Set(ref _packagings, value);
        }
        
        
        public ProductForSaleViewModel(IProductForSaleService service,
            IProductService productService,
            IStoreService storeService,
            IProductDeliveryService productDeliveryService,
            IPackagingService packagingService)
        {
            _service = service;
            _productService = productService;
            _storeService = storeService;
            _productDeliveryService = productDeliveryService;
            _packagingService = packagingService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            
            _productForSales = new ObservableCollection<ProductForSale>();
            OnGetCommandExecuted(null);
            
        }

        private async Task LoadComboBoxes()
        { 
            await GetProduct();
            await GetStores();
            await GetProductDeliveries();
            await GetPackagings();
        }
        
        private async Task GetProduct()
        {
            Products = new ObservableCollection<Product>(await _productService.Get());
        }
        
        private async Task GetStores()
        {
            Stores = new ObservableCollection<Store>(await _storeService.Get());
        }
        
        private async Task GetProductDeliveries()
        {
            ProductDeliveries = new ObservableCollection<ProductDelivery>(await _productDeliveryService.Get());
        }
        
        private async Task GetPackagings()
        {
            Packagings = new ObservableCollection<Packaging>(await _packagingService.Get());
        }
    }
}
