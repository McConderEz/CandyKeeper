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
using CandyKeeper.Presentation.Extensions;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;
using CandyKeeper.Presentation.Views.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using User = CandyKeeper.Presentation.Models.User;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ProductForSaleViewModel : ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly IProductForSaleService _service;
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
        private readonly IProductDeliveryService _productDeliveryService;
        private readonly IPackagingService _packagingService;
        private readonly IUserService _userService;
        private readonly IUserSessionService _userSessionService;
        private readonly IConfiguration _configuration;
        private User _currentUser;

        private readonly List<DatabaseRole> _roles;
        
        private ObservableCollection<ProductForSale> _productForSales;
        
        private ObservableCollection<Product> _products;
        private ObservableCollection<Store> _stores;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<ProductDelivery> _productDeliveries;
        private ObservableCollection<Packaging> _packagings;
        private ObservableCollection<ProductType> _productTypes;

        private Models.ProductForSale _selectedItem = new();
        private ProductForSale _selectedItemForDetails;

        private DetailsProductForSalePage _detailsView;
        private string _searchingString;
        
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
                    if(p == null)
                        ProductForSales = new ObservableCollection<ProductForSale>(await _service.Get());
                }
                else
                {
                    if(p == null)
                        ProductForSales = new ObservableCollection<ProductForSale>(await _service.GetByStoreId((int)CurrentUser.StoreId));
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
                        _selectedItem.Volume);

                    if (productForSale.IsFailure)
                        throw new ArgumentException();

                    await _service.Create(productForSale.Value);
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
                        _selectedItem.Volume);

                    if (productForSale.IsFailure)
                        throw new ArgumentException();

                    await _service.Update(productForSale.Value);
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
                    ProductForSales = new ObservableCollection<ProductForSale>(await _service.GetBySearchingString(SearchingString));
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
        
        
        #region FilterShowCommand

        public ICommand FilterShowCommand { get; }
        private bool CanFilterShowCommandExecute(object p) => true;
        public async void OnFilterShowCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                await GetSuppliers();
                Stores = new ObservableCollection<Store>(await _storeService.Get());
                ProductDeliveries = new ObservableCollection<ProductDelivery>(await _productDeliveryService.Get());
                await GetPackagings();
                await GetProductType();
                var page = new FilterProductForSalePage();
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

                var productForSales = await _service.Get();
                
                ProductForSales = Filter(MinPrice,
                    MaxPrice,
                    MinVolume,
                    MaxVolume,
                    SelectedStoreIds.ToList(),
                    SelectedSupplierIds.ToList(),
                    SelectedProductDeliveryIds.ToList(),
                    SelectedPackagingIds.ToList(),
                    SelectedProductTypeIds.ToList(),
                    productForSales);

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
                if (p is Store store)
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
                else if (p is Supplier supplier)
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
                else if (p is ProductDelivery productDelivery)
                {
                    if (SelectedProductDeliveryIds.Contains(productDelivery.Id))
                    {
                        SelectedProductDeliveryIds.Remove(productDelivery.Id);
                    }
                    else
                    {
                        SelectedProductDeliveryIds.Add(productDelivery.Id);
                    }
                }
                else if (p is Packaging packaging)
                {
                    if (SelectedPackagingIds.Contains(packaging.Id))
                    {
                        SelectedPackagingIds.Remove(packaging.Id);
                    }
                    else
                    {
                        SelectedPackagingIds.Add(packaging.Id);
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

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
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

        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set => Set(ref _productTypes, value);
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
        
        public User CurrentUser
        {
            get => _currentUser;
            set => Set(ref _currentUser, value);
        }
        
        public bool IsAdminVisible => IsAdmin();
        public bool IsClientVisible => IsClient();
        public bool IsManagerVisible => IsManager();
        
        //TODO: Решить проблему с MemoryCache
        public ProductForSaleViewModel(IProductForSaleService service,
            IProductService productService,
            IStoreService storeService,
            ISupplierService supplierService,
            IProductDeliveryService productDeliveryService,
            IPackagingService packagingService,
            IProductTypeService productTypeService,
            IConfiguration configuration,
            IUserService userService,
            IUserSessionService userSessionService)
        {
            MainWindowsViewModel.TransferCurrentUserEvent += GetCurrentUser;
            _service = service;
            _productService = productService;
            _storeService = storeService;
            _supplierService = supplierService;
            _productDeliveryService = productDeliveryService;
            _packagingService = packagingService;
            _productTypeService = productTypeService;
            _userService = userService;
            _userSessionService = userSessionService;
            _configuration = configuration;

            //CurrentUser = _userSessionService.GetUserData<User>("CurrentUser");
            
            CurrentUser = CurrentUserTransfer.CurrentUser;
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            FilterShowCommand = new LambdaCommand(OnFilterShowCommandExecuted);
            FilterCommand = new LambdaCommand(OnFilterCommandExecuted);
            ToggleSelectionCommand = new LambdaCommand(OnToggleSelectionCommandExecuted);
            
            _productForSales = new ObservableCollection<ProductForSale>();

            SelectedStoreIds = new ObservableCollection<int>();
            SelectedSupplierIds = new ObservableCollection<int>();
            SelectedProductDeliveryIds = new ObservableCollection<int>();
            SelectedPackagingIds = new ObservableCollection<int>();
            SelectedProductTypeIds = new ObservableCollection<int>();
            
            _roles = GetDatabaseRoles();
            OnGetCommandExecuted(null);
        }

        private void GetCurrentUser(object? sender, User e)
        {
            CurrentUser = e;
            OnPropertyChanged(nameof(IsAdminVisible));
            OnPropertyChanged(nameof(IsClientVisible));
            OnPropertyChanged(nameof(IsManagerVisible));
        }

        private async Task LoadComboBoxes()
        { 
            await GetProduct();
            await GetStores();
            await GetProductDeliveries();
            await GetPackagings();
            await GetSuppliers();
        }
        
        private async Task GetProduct()
        {
            Products = new ObservableCollection<Product>(await _productService.Get());
        }

        private async Task GetProductType()
        {
            ProductTypes = new ObservableCollection<ProductType>(await _productTypeService.Get());
        }
        
        private async Task GetStores()
        {
            Store storeOfCurrentUser = await _storeService.GetById((int)CurrentUser.StoreId);
            Stores = new ObservableCollection<Store>();
            
            Stores.Add(storeOfCurrentUser);
        }
        
        private async Task GetProductDeliveries()
        {
            var pdFiltered = await _productDeliveryService.Get();
            ProductDeliveries = new ObservableCollection<ProductDelivery>(pdFiltered.Where(pd => pd.StoreId == CurrentUser.StoreId));
        }
        
        private async Task GetPackagings()
        {
            Packagings = new ObservableCollection<Packaging>(await _packagingService.Get());
        }

        private async Task GetSuppliers()
        {
            Suppliers = new ObservableCollection<Supplier>(await _supplierService.Get());
        }
        
        public bool IsAdmin()
        {
            return CurrentUser != null && _roles.Any(r => r.Name == "Admin" && r.PrincipalId == CurrentUser.PrincipalId);
        }

        public bool IsClient()
        {
            return CurrentUser != null && _roles.Any(r => r.Name == "Client" && r.PrincipalId == CurrentUser.PrincipalId);
        }

        public bool IsManager()
        {
            return CurrentUser != null && _roles.Any(r => r.Name == "Manager" && r.PrincipalId == CurrentUser.PrincipalId);
        }
        
        private List<DatabaseRole> GetDatabaseRoles()
        {
            string query = "SELECT principal_id, name FROM sys.database_principals WHERE type = 'R'";
            var roles = new List<DatabaseRole>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var role = new DatabaseRole
                        {
                            PrincipalId = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        roles.Add(role);
                    }
                }
            }

            return roles;
        }
    
        private class DatabaseRole
        {
            public int PrincipalId { get; set; }
            public string Name { get; set; }
        }
        
        
        #region Поля_фильтрации

        private int? _minPrice;
        private int? _maxPrice;
        private int? _minVolume;
        private int? _maxVolume;
        private ObservableCollection<int> _selectedStoreIds;
        private ObservableCollection<int> _selectedSupplierIds;
        private ObservableCollection<int> _selectedProductDeliveryIds;
        private ObservableCollection<int> _selectedProductTypeIds;
        private ObservableCollection<int> _selectedPackagingIds;
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public int? MinPrice
        {
            get => _minPrice;
            set => Set(ref _minPrice, value);
        }
        
        public int? MaxPrice
        {
            get => _maxPrice;
            set => Set(ref _maxPrice, value);
        }
        
        public int? MinVolume
        {
            get => _minVolume;
            set => Set(ref _minVolume, value);
        }
        
        public int? MaxVolume
        {
            get => _maxVolume;
            set => Set(ref _maxVolume, value);
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
        
        public ObservableCollection<int> SelectedProductDeliveryIds
        {
            get => _selectedProductDeliveryIds;
            set => Set(ref _selectedProductDeliveryIds, value);
        }
        
        public ObservableCollection<int> SelectedProductTypeIds
        {
            get => _selectedProductTypeIds;
            set => Set(ref _selectedProductTypeIds, value);
        }
        
        public ObservableCollection<int> SelectedPackagingIds
        {
            get => _selectedPackagingIds;
            set => Set(ref _selectedPackagingIds, value);
        }

        private void Clear()
        {
            _selectedStoreIds.Clear();
            _selectedSupplierIds.Clear();
            _selectedProductDeliveryIds.Clear();
            _selectedProductTypeIds.Clear();
            _selectedPackagingIds.Clear();
        }
        
        #endregion
        
        //TODO: Протестировать
        private static ObservableCollection<ProductForSale>? Filter(int? minPrice = 0, 
            int? maxPrice = Int32.MaxValue,
            int? minVolume = 0, 
            int? maxVolume = Int32.MaxValue,
            List<int> storeIds = null,
            List<int> supplierIds = null ,
            List<int> productDeliveryIds = null,
            List<int> packagingIds = null,
            List<int> productTypeIds = null,
            List<ProductForSale> productForSales = null)
        {
            if (minPrice.HasValue)
                productForSales = productForSales.Where(a => a.Price >= minPrice.Value).ToList();
            if (maxPrice.HasValue)
                productForSales = productForSales.Where(a => a.Price <= maxPrice.Value).ToList();
            if (minVolume.HasValue)
                productForSales = productForSales.Where(a => a.Volume >= minVolume.Value).ToList();
            if (maxVolume.HasValue)
                productForSales = productForSales.Where(a => a.Volume <= maxVolume.Value).ToList();
            if (storeIds != null && storeIds.Any())
                productForSales = productForSales.Where(a => storeIds.Contains(a.StoreId)).ToList();
            if (supplierIds != null && supplierIds.Any())
                productForSales = productForSales.Where(a => supplierIds.Contains(a.ProductDelivery.SupplierId)).ToList();
            if (productDeliveryIds != null && productDeliveryIds.Any())
                productForSales = productForSales.Where(a => productDeliveryIds.Contains(a.ProductDeliveryId)).ToList();
            if (packagingIds != null && packagingIds.Any())
                productForSales = productForSales.Where(a => packagingIds.Contains(a.PackagingId)).ToList();
            if (productTypeIds != null && productTypeIds.Any())
                productForSales = productForSales.Where(a => productTypeIds.Contains(a.Product.ProductTypeId)).ToList();
            
            return new ObservableCollection<ProductForSale>(productForSales);
        }
    }
}
