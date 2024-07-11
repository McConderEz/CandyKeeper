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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;
using CandyKeeper.Presentation.Views.Windows;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductForSaleViewModel : ViewModel
    {
        public delegate void RefreshDataDelegate(object p);
        public delegate void CloseUserControlDelegate(object p);
        private static event RefreshDataDelegate _refreshEvent;
        private static event CloseUserControlDelegate _closeEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
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
        private ProductForSale _selectedItemForDetails;

        private DetailsProductForSalePage _detailsView;
        
        #region Команды

        #region  GetCommand

        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                ProductForSales = new ObservableCollection<ProductForSale>(await _service.Get());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        #endregion

        #region AddEditShowCommand
        public ICommand AddEditShowCommand { get; }
        private bool CanAddEditShowCommandExecute(object p) => true;
        public async void OnAddEditShowCommandExecuted(object p)
        {
            if (p is int id)
            {
                SelectedItem.Id = id;
            }

            await _semaphore.WaitAsync();

            try
            {
                await LoadComboBoxes();
                var page = new AddEditProductForSalePage();
                page.DataContext = this;
                page.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        #endregion

        #region AddEditCommand
        public ICommand AddEditCommand { get; }
        private bool CanAddEditCommandExecute(object p) => true;
        public async void OnAddEditCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
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
                
                _refreshEvent?.Invoke(null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        

        #endregion

        #region DeleteCommand

        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;

        public async void OnDeleteCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    await _service.Delete(id);
                    _refreshEvent?.Invoke(null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #endregion

        #region DetailsCommand
        public ICommand DetailsCommand { get; }
        private bool CanDetailsCommandExecute(object p) => true;

        public async void OnDetailsCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    SelectedItemForDetails = await _service.GetById(id);
                    _closeEvent?.Invoke(null);
                    DetailsPage = new DetailsProductForSalePage();
                    DetailsPage.DataContext = this;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        

        #endregion
        
        #endregion
        

        public static event RefreshDataDelegate RefreshEvent
        {
            add => _refreshEvent += value;
            remove => _refreshEvent -= value;
        }

        public static event CloseUserControlDelegate CloseEvent
        {
            add => _closeEvent += value;
            remove => _closeEvent -= value;
        }
        
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
        public Models.ProductForSale SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public ProductForSale SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsProductForSalePage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
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
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            
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
