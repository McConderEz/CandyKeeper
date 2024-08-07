﻿using CandyKeeper.Application.Interfaces;
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
using CandyKeeper.Presentation.Extensions;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductDeliveryViewModel: ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly IProductDeliveryService _service;
        private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
        private readonly IProductForSaleService _productForSaleService;
        private readonly IProductTypeService _productTypeService;
        private readonly IProductService _productService;
        
        private ObservableCollection<ProductDelivery> _productDeliveries;
        
        private ObservableCollection<Store> _stores;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<ProductForSale> _productForSales;
        private ObservableCollection<Product> _products;
        private ObservableCollection<ProductType> _productTypes;

        private Models.ProductDelivery _selectedItem = new();
        private ProductDelivery _selectedItemForDetails;
        private Models.User _currentUser;
        private int _productForSaleId;

        private DetailsProductDeliveryPage _detailsView;
        private string _searchingString;
        private DateTime _displayDate = new DateTime(2000, 1, 1);
        public DateTime DisplayDate
        {
            get => _displayDate;
            set => Set(ref _displayDate, value);
        }
        
        #region Команды

        #region  GetCommand

        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (CurrentUser.StoreId == null)
                {
                    if (p == null)
                    {
                        ProductDeliveries = new ObservableCollection<ProductDelivery>(await _service.Get());
                    }
                }
                else
                {
                    if (p == null)
                    {
                        ProductDeliveries =
                            new ObservableCollection<ProductDelivery>(
                                await _service.GetByStoreId((int)CurrentUser.StoreId));
                    }
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
                var page = new AddEditProductDeliveryPage();
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
                    var productDelivery = ProductDelivery.Create(
                        _selectedItem.Id,
                        _selectedItem.DeliveryDate,
                        _selectedItem.SupplierId,
                        _selectedItem.StoreId);

                    if (productDelivery.IsFailure)
                        throw new ArgumentException();

                    await _service.Create(productDelivery.Value);
                }
                else
                {
                    var productDelivery = ProductDelivery.Create(
                        _selectedItem.Id,
                        _selectedItem.DeliveryDate,
                        _selectedItem.SupplierId,
                        _selectedItem.StoreId);

                    if (productDelivery.IsFailure)
                        throw new ArgumentException();

                    await _service.Update(productDelivery.Value);
                }

                _refreshEvent?.Invoke(null);
            }
            catch (ArgumentException)
            {
                IsInvalid = true;
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
                    DetailsPage = new DetailsProductDeliveryPage();
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
        
        #region ReturnCommand
        
        public ICommand ReturnCommand { get; }
        private bool CanReturnCommandExecute(object p) => true;

        public async void OnReturnCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                _returnEvent?.Invoke(null);
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

        #region SearchingCommand

        public ICommand SearchCommand { get; }
        private bool CanSearchCommandExecute(object p) => true;
        public async void OnSearchCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(SearchingString))
                {
                    ProductDeliveries = new ObservableCollection<ProductDelivery>(await _service.GetBySearchingString(SearchingString));
                }
                else
                {
                    OnGetCommandExecuted(null);
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
        
        #region AddProductInProductDeliveryShowCommand
        public ICommand AddProductInProductDeliveryShowCommand { get; }
        private bool CanAAddProductInProductDeliveryShowCommandExecute(object p) => true;
        public async void OnAddProductInProductDeliveryShowCommandExecuted(object p)
        {
            if (p is int id)
            {
                SelectedItem.Id = id;
            }

            await _semaphore.WaitAsync();

            try
            {
                await GetProductForSales();
                var page = new AddProductInProductDeliveryPage();
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

        #region AddProductInProductDeliveryCommand
        public ICommand AddProductInProductDeliveryCommand { get; }
        private bool CanAddProductInProductDeliveryCommandExecute(object p) => true;
        public async void OnAddProductInProductDeliveryCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    var productForSale = await _productForSaleService.GetById(_productForSaleId);
                    await _service.AddProductForSale(_selectedItemForDetails.Id, productForSale);
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
        
        #region FilterShowCommand

        public ICommand FilterShowCommand { get; }
        private bool CanFilterShowCommandExecute(object p) => true;
        public async void OnFilterShowCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                await GetSuppliers();
                await GetStores();
                await GetProductTypes();
                await GetProductForSales();
                await GetProduct();
                
                var page = new FilterProductDelivryPage();
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

        #region FilterCommand

        public ICommand FilterCommand { get; }
        private bool CanFilterCommandExecute(object p) => true;
        public async void OnFilterCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                List<ProductDelivery> productDeliveries;
                if (CurrentUser.StoreId != null)
                {
                    productDeliveries = new List<ProductDelivery>(
                        await _service.GetByStoreId((int)CurrentUser.StoreId));
                }
                else
                {
                    productDeliveries = await _service.Get();
                }

                ProductDeliveries = Filter(
                    MinDeliveryDate,
                    MaxDeliveryDate,
                    SelectedSupplierIds.ToList(),
                    SelectedProductTypeIds.ToList(),
                    SelectedStoreIds.ToList(),
                    SelectedProductIds.ToList(),
                    productDeliveries);

                _refreshEvent?.Invoke(true);
            }
            catch (ArgumentException)
            {
                IsInvalid = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Clear();
                _semaphore.Release();
            }
        }

        #endregion

        #region ToggleSelectionCommand

        public ICommand ToggleSelectionCommand { get; }
        private bool CanToggleSelectionCommandExecute(object p) => true;
        public async void OnToggleSelectionCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            { 
                if (p is Supplier supplier)
                {
                    if (SelectedSupplierIds.Contains(supplier.Id))
                    {
                        SelectedSupplierIds.Remove(supplier.Id);
                    }
                    else
                    {
                        SelectedSupplierIds.Add(supplier.Id);
                    }
                }
                else if (p is Store store)
                {
                    if (SelectedStoreIds.Contains(store.Id))
                    {
                        SelectedStoreIds.Remove(store.Id);
                    }
                    else
                    {
                        SelectedStoreIds.Add(store.Id);
                    }
                }
                else if (p is Product product)
                {
                    if (SelectedProductIds.Contains(product.Id))
                    {
                        SelectedProductIds.Remove(product.Id);
                    }
                    else
                    {
                        SelectedProductIds.Add(product.Id);
                    }
                }
                else if (p is ProductType productType)
                {
                    if (SelectedProductTypeIds.Contains(productType.Id))
                    {
                        SelectedProductTypeIds.Remove(productType.Id);
                    }
                    else
                    {
                        SelectedProductTypeIds.Add(productType.Id);
                    }
                }
            }
            catch (ArgumentException)
            {
                IsInvalid = true;
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
        

        private bool _isInvalid = false;

        public bool IsInvalid
        {
            get => _isInvalid;
            set => Set(ref _isInvalid, value);
        }

        public Models.User CurrentUser
        {
            get => _currentUser;
            set => Set(ref _currentUser, value);
        }
        
        public string SearchingString
        {
            get => _searchingString;
            set => Set(ref _searchingString, value);
        }
        
        public static event Delegate RefreshEvent
        {
            add => _refreshEvent += value;
            remove => _refreshEvent -= value;
        }

        public static event Delegate CloseEvent
        {
            add => _closeEvent += value;
            remove => _closeEvent -= value;
        }
        
        public static event Delegate ReturnEvent
        {
            add => _returnEvent += value;
            remove => _returnEvent -= value;
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
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
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
        
        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set => Set(ref _productTypes, value);
        }
        
        public Models.ProductDelivery SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public ProductDelivery SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsProductDeliveryPage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }

        public int ProductForSaleId
        {
            get => _productForSaleId;
            set => Set(ref _productForSaleId, value);
        }
        

        public ProductDeliveryViewModel(IProductDeliveryService service,
            IStoreService storeService, 
            ISupplierService supplierService,
            IProductForSaleService productForSaleService,
            IProductTypeService productTypeService,
            IProductService productService)
        {
            _service = service;
            _storeService = storeService;
            _supplierService = supplierService;
            _productForSaleService = productForSaleService;
            _productTypeService = productTypeService;
            _productService = productService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            AddProductInProductDeliveryShowCommand = new LambdaCommand(OnAddProductInProductDeliveryShowCommandExecuted);
            AddProductInProductDeliveryCommand = new LambdaCommand(OnAddProductInProductDeliveryCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            FilterShowCommand = new LambdaCommand(OnFilterShowCommandExecuted);
            FilterCommand = new LambdaCommand(OnFilterCommandExecuted);
            ToggleSelectionCommand = new LambdaCommand(OnToggleSelectionCommandExecuted);

            CurrentUser = CurrentUserTransfer.CurrentUser;
            
            _productDeliveries = new ObservableCollection<ProductDelivery>();
            SelectedStoreIds = new ObservableCollection<int>();
            SelectedSupplierIds = new ObservableCollection<int>();
            SelectedProductIds = new ObservableCollection<int>();
            SelectedProductTypeIds = new ObservableCollection<int>();
            OnGetCommandExecuted(null);
        }
        
        private async Task LoadComboBoxes()
        { 
            await GetStores();
            await GetSuppliers();
        }
        
        
        private async Task GetStores()
        {
            Stores = new ObservableCollection<Store>(await _storeService.Get());
        }
        
        
        private async Task GetSuppliers()
        {
            Suppliers = new ObservableCollection<Supplier>(await _supplierService.Get());
        }

        private async Task GetProductForSales()
        {
            ProductForSales = new ObservableCollection<ProductForSale>(await _productForSaleService.Get());
        }
        
        private async Task GetProduct()
        {
            Products = new ObservableCollection<Product>(await _productService.Get());
        }
        
        private async Task GetProductTypes()
        {
            ProductTypes = new ObservableCollection<ProductType>(await _productTypeService.Get());
        }
        
        #region Поля_фильтрации
        
        private DateTime? _minDeliveryDate;
        private DateTime? _maxDeliveryDate;
        private ObservableCollection<int> _selectedStoreIds;
        private ObservableCollection<int> _selectedSupplierIds;
        private ObservableCollection<int> _selectedProductIds;
        private ObservableCollection<int> _selectedProductTypeIds;
        

        public DateTime? MinDeliveryDate
        {
            get => _minDeliveryDate;
            set => Set(ref _minDeliveryDate, value);
        }
        
        public DateTime? MaxDeliveryDate
        {
            get => _maxDeliveryDate;
            set => Set(ref _maxDeliveryDate, value);
        }
        
        public ObservableCollection<int> SelectedStoreIds
        {
            get => _selectedStoreIds;
            set => Set(ref _selectedStoreIds, value); 
        }
        
        public ObservableCollection<int> SelectedSupplierIds
        {
            get => _selectedSupplierIds;
            set => Set(ref _selectedSupplierIds, value);
        }
        
        public ObservableCollection<int> SelectedProductTypeIds
        {
            get => _selectedProductTypeIds;
            set => Set(ref _selectedProductTypeIds, value);
        }
        
        public ObservableCollection<int> SelectedProductIds
        {
            get => _selectedProductIds;
            set => Set(ref _selectedProductIds, value);
        }
        
       

        private void Clear()
        {
            _selectedStoreIds.Clear();
            _selectedSupplierIds.Clear();
            _selectedProductTypeIds.Clear();
            _selectedProductIds.Clear();
        }
        
        #endregion
        
        //TODO: Протестировать
        private static ObservableCollection<ProductDelivery>? Filter(
            DateTime? minDeliveryDate = null,
            DateTime? maxDeliveryDate  = null,
            List<int> supplierIds = null ,
            List<int> productTypeIds = null,
            List<int> storeIds = null,
            List<int> productIds = null,
            List<ProductDelivery> productDeliveries = null)
        {
            if (minDeliveryDate.HasValue)
                productDeliveries = productDeliveries.Where(a => a.DeliveryDate >= minDeliveryDate.Value).ToList();
            if (maxDeliveryDate.HasValue)
                productDeliveries = productDeliveries.Where(a => a.DeliveryDate <= maxDeliveryDate.Value).ToList();
            if (supplierIds != null && supplierIds.Any())
                productDeliveries = productDeliveries.Where(a => supplierIds.Contains(a.SupplierId)).ToList();
            if (productTypeIds != null && productTypeIds.Any())
                productDeliveries = productDeliveries.Where(a => a.ProductForSales.Any(pfs => productTypeIds.Contains(pfs.Product.ProductTypeId))).ToList();
            if (storeIds != null && storeIds.Any())
                productDeliveries = productDeliveries.Where(a => storeIds.Contains(a.StoreId)).ToList();
            if (productIds != null && productIds.Any())
                productDeliveries = productDeliveries.Where(a => a.ProductForSales.Any(pfs => productIds.Contains(pfs.ProductId))).ToList();
            
            return new ObservableCollection<ProductDelivery>(productDeliveries);
        }
    }
}
