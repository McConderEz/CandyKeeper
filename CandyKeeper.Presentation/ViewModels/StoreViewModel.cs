using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CandyKeeper.Application.Services;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class StoreViewModel : ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly IProductDeliveryService _productDeliveryService;
        private readonly IStoreService _service;
        private readonly ISupplierService _supplierService;
        private readonly IProductForSaleService _productForSaleService;
        private readonly IOwnershipTypeService _ownershipTypeService;
        private readonly IDistrictService _districtService;
        private readonly IPackagingService _packagingService;
        private readonly IProductTypeService _productTypeService;
        private readonly ICityService _cityService;
        
        private ObservableCollection<ProductDelivery> _productDeliveries;
        private ObservableCollection<Store> _stores;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<ProductForSale> _productForSales;
        private ObservableCollection<OwnershipType> _ownershipTypes;
        private ObservableCollection<District> _districts;
        private ObservableCollection<City> _cities;
        private ObservableCollection<ProductType> _productTypes;
        private ObservableCollection<Packaging> _packagings;


        private Models.Store _selectedItem = new();
        private Store _selectedItemForDetails;

        private int _supplierId;

        private ComboBoxItem _selectedDataGridInDetailsPage;
        
        private DetailsStorePage _detailsView;
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
                if(p == null)
                    Stores = new ObservableCollection<Store>(await _service.Get());
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
                var page = new AddEditStorePage();
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
                    var store = Store.Create(
                        _selectedItem.Id,
                        _selectedItem.StoreNumber,
                        _selectedItem.Name,
                        _selectedItem.YearOfOpened,
                        _selectedItem.Phone,
                        _selectedItem.OwnershipTypeId,
                        _selectedItem.DistrictId
                    );

                    if (store.IsFailure)
                        throw new ArgumentException();

                    await _service.Create(store.Value);
                }
                else
                {
                    var store = Store.Create(
                        _selectedItem.Id,
                        _selectedItem.StoreNumber,
                        _selectedItem.Name,
                        _selectedItem.YearOfOpened,
                        _selectedItem.Phone,
                        _selectedItem.OwnershipTypeId,
                        _selectedItem.DistrictId);

                    if (store.IsFailure)
                        throw new ArgumentException();

                    await _service.Update(store.Value);
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
                    DetailsPage = new DetailsStorePage();
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
                    Stores = new ObservableCollection<Store>(await _service.GetBySearchingString(SearchingString));
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
        
        #region AddSupplierInStoreShowCommand
        public ICommand AddSupplierInStoreShowCommand { get; }
        private bool CanAddSupplierInStoreShowCommandExecute(object p) => true;
        public async void OnAddSupplierInStoreShowCommandExecuted(object p)
        {
            if (p is int id)
            {
                SelectedItem.Id = id;
            }

            await _semaphore.WaitAsync();

            try
            {
                await GetSuppliers();
                var page = new AddSupplierInStorePage();
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

        #region AddSupplierInStoreCommand
        public ICommand AddSupplierInStoreCommand { get; }
        private bool CanAddSupplierInStoreCommandExecute(object p) => true;
        public async void OnAddSupplierInStoreCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    var supplier = await _supplierService.GetById(_supplierId);
                    await _service.AddSupplier(_selectedItemForDetails.Id, supplier);
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

        #region DeleteSupplierFromStoreCommand

        public ICommand DeleteSupplierFromStoreCommand { get; }
        private bool CanDeleteSupplierFromStoreCommadExecuted(object p) => true;
        public async void OnDeleteSupplierFromStoreCommadExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    var supplier = await _supplierService.GetById(id);
                    await _service.DeleteSupplier(_selectedItemForDetails.Id, supplier);
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
                await GetProductDeliveries();
                await GetPackagings();
                await GetProductType();
                await GetCities();
                await GetDistricts();
                await GetOwnershipTypes();
                
                var page = new FilterStorePage();
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

                var stores = await _service.Get();
                
                Stores = Filter(MinStoreNumber,
                    MaxStoreNumber,
                    MinNumberOfEmployee,
                    MaxNumberOfEmployee,
                    MinYearOfOpened,
                    MaxYearOfOpened,
                    SelectedSupplierIds.ToList(),
                    SelectedProductDeliveryIds.ToList(),
                    SelectedPackagingIds.ToList(),
                    SelectedProductTypeIds.ToList(),
                    SelectedDistrictIds.ToList(),
                    SelectedCityIds.ToList(),
                    _selectedOwnershipTypeIds.ToList(),
                    stores);

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
                if (p is City city)
                {
                    if (SelectedCityIds.Contains(city.Id))
                    {
                        SelectedCityIds.Remove(city.Id);
                    }
                    else
                    {
                        SelectedCityIds.Add(city.Id);
                    }
                }
                else if (p is District district)
                {
                    if (SelectedDistrictIds.Contains(district.Id))
                    {
                        SelectedDistrictIds.Remove(district.Id);
                    }
                    else
                    {
                        SelectedDistrictIds.Add(district.Id);
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
                else if (p is OwnershipType ownershipType)
                {
                    if (SelectedOwnershipTypeIds.Contains(ownershipType.Id))
                    {
                        SelectedOwnershipTypeIds.Remove(ownershipType.Id);
                    }
                    else
                    {
                        SelectedOwnershipTypeIds.Add(ownershipType.Id);
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
        
        public ObservableCollection<OwnershipType> OwnershipTypes
        {
            get => _ownershipTypes;
            set => Set(ref _ownershipTypes, value);
        }
        
        public ObservableCollection<District> Districts
        {
            get => _districts;
            set => Set(ref _districts, value);
        }
        
        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
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
        
        public Models.Store SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public Store SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsStorePage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }

        public int SupplierId
        {
            get => _supplierId;
            set => Set(ref _supplierId, value);
        }

        public ComboBoxItem SelectedDataGridInDetailsPage
        {
            get => _selectedDataGridInDetailsPage;
            set => Set(ref _selectedDataGridInDetailsPage, value);
        }
        

        public StoreViewModel(IProductDeliveryService productDeliveryService,
            IStoreService service, 
            ISupplierService supplierService,
            IProductForSaleService productForSaleService,
            IOwnershipTypeService ownershipTypeService,
            IDistrictService districtService,
            IPackagingService packagingService,
            IProductTypeService productTypeService,
            ICityService cityService)
        {
            _service = service;
            _productDeliveryService = productDeliveryService;
            _supplierService = supplierService;
            _productForSaleService = productForSaleService;
            _ownershipTypeService = ownershipTypeService;
            _districtService = districtService;
            _packagingService = packagingService;
            _productTypeService = productTypeService;
            _cityService = cityService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            AddSupplierInStoreShowCommand = new LambdaCommand(OnAddSupplierInStoreShowCommandExecuted);
            AddSupplierInStoreCommand = new LambdaCommand(OnAddSupplierInStoreCommandExecuted);
            DeleteSupplierFromStoreCommand = new LambdaCommand(OnDeleteSupplierFromStoreCommadExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            FilterShowCommand = new LambdaCommand(OnFilterShowCommandExecuted);
            FilterCommand = new LambdaCommand(OnFilterCommandExecuted);
            ToggleSelectionCommand = new LambdaCommand(OnToggleSelectionCommandExecuted);
            
            _productDeliveries = new ObservableCollection<ProductDelivery>();
            
            SelectedStoreIds = new ObservableCollection<int>();
            SelectedSupplierIds = new ObservableCollection<int>();
            SelectedProductDeliveryIds = new ObservableCollection<int>();
            SelectedPackagingIds = new ObservableCollection<int>();
            SelectedProductTypeIds = new ObservableCollection<int>();
            SelectedCityIds = new ObservableCollection<int>();
            SelectedDistrictIds = new ObservableCollection<int>();
            SelectedOwnershipTypeIds = new ObservableCollection<int>();
            
            OnGetCommandExecuted(null);
        }
        
        private async Task LoadComboBoxes()
        { 
            await GetSuppliers();
            await GetOwnershipTypes();
            await GetDistricts();
            await GetProductForSales();
        }
        
        
        private async Task GetStores()
        {
            Stores = new ObservableCollection<Store>(await _service.Get());
        }
        
        
        private async Task GetSuppliers()
        {
            Suppliers = new ObservableCollection<Supplier>(await _supplierService.Get());
        }

        private async Task GetProductForSales()
        {
            ProductForSales = new ObservableCollection<ProductForSale>(await _productForSaleService.Get());
        }

        private async Task GetOwnershipTypes()
        {
            OwnershipTypes = new ObservableCollection<OwnershipType>(await _ownershipTypeService.Get());
        }
        
        private async Task GetDistricts()
        {
            Districts = new ObservableCollection<District>(await _districtService.Get());
        }
        private async Task GetCities()
        {
            Cities = new ObservableCollection<City>(await _cityService.Get());
        }
        
        private async Task GetPackagings()
        {
            Packagings = new ObservableCollection<Packaging>(await _packagingService.Get());
        }

        private async Task GetProductType()
        {
            ProductTypes = new ObservableCollection<ProductType>(await _productTypeService.Get());
        }
        
        private async Task GetProductDeliveries()
        {
            ProductDeliveries = new ObservableCollection<ProductDelivery>(await _productDeliveryService.Get());
        }
        
        #region Поля_фильтрации

        private int? _minStoreNumber;
        private int? _maxStoreNumber;
        private int? _minNumberOfEmployee;
        private int? _maxNumberOfEmployee;
        private DateTime? _minYearOfOpened;
        private DateTime? _maxYearOfOpened;
        private ObservableCollection<int> _selectedStoreIds;
        private ObservableCollection<int> _selectedSupplierIds;
        private ObservableCollection<int> _selectedProductDeliveryIds;
        private ObservableCollection<int> _selectedProductTypeIds;
        private ObservableCollection<int> _selectedPackagingIds;
        private ObservableCollection<int> _selectedDistrictIds;
        private ObservableCollection<int> _selectedCityIds;
        private ObservableCollection<int> _selectedOwnershipTypeIds;
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public int? MinStoreNumber
        {
            get => _minStoreNumber;
            set => Set(ref _minStoreNumber, value);
        }
        
        public int? MaxStoreNumber
        {
            get => _maxStoreNumber;
            set => Set(ref _maxStoreNumber, value);
        }
        
        public int? MinNumberOfEmployee
        {
            get => _minNumberOfEmployee;
            set => Set(ref _minNumberOfEmployee, value);
        }
        
        public int? MaxNumberOfEmployee
        {
            get => _maxNumberOfEmployee;
            set => Set(ref _maxNumberOfEmployee, value);
        }

        public DateTime? MinYearOfOpened
        {
            get => _minYearOfOpened;
            set => Set(ref _minYearOfOpened, value);
        }
        
        public DateTime? MaxYearOfOpened
        {
            get => _maxYearOfOpened;
            set => Set(ref _maxYearOfOpened, value);
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
        
        public ObservableCollection<int> SelectedDistrictIds
        {
            get => _selectedDistrictIds;
            set => Set(ref _selectedDistrictIds, value);
        }
        
        public ObservableCollection<int> SelectedCityIds
        {
            get => _selectedCityIds;
            set => Set(ref _selectedCityIds, value);
        }
        
        public ObservableCollection<int> SelectedOwnershipTypeIds
        {
            get => _selectedOwnershipTypeIds;
            set => Set(ref _selectedOwnershipTypeIds, value);
        }
        #endregion
        
        //TODO: Протестировать
        private static ObservableCollection<Store>? Filter(int? minStoreNumber = 100000, 
            int? maxStoreNumber = Int32.MaxValue,
            int? minNumberOfEmployes = 0, 
            int? maxNumberOfEmployes = Int32.MaxValue,
            DateTime? minYearOfOpened = null,
            DateTime? maxYearOfOpened = null,
            List<int> supplierIds = null ,
            List<int> productDeliveryIds = null,
            List<int> packagingIds = null,
            List<int> productTypeIds = null,
            List<int> districtIds = null,
            List<int> cityIds = null,
            List<int> ownershipTypeIds = null,
            List<Store> stores = null)
        {
            if (minStoreNumber.HasValue)
                stores = stores.Where(a => a.StoreNumber >= minStoreNumber.Value).ToList();
            if (maxStoreNumber.HasValue)
                stores = stores.Where(a => a.StoreNumber <= maxStoreNumber.Value).ToList();
            if (minNumberOfEmployes.HasValue)
                stores = stores.Where(a => a.NumberOfEmployees >= minNumberOfEmployes.Value).ToList();
            if (maxNumberOfEmployes.HasValue)
                stores = stores.Where(a => a.NumberOfEmployees <= maxNumberOfEmployes.Value).ToList();
            if (minYearOfOpened.HasValue)
                stores = stores.Where(a => a.YearOfOpened >= minYearOfOpened.Value).ToList();
            if (maxYearOfOpened.HasValue)
                stores = stores.Where(a => a.YearOfOpened <= maxYearOfOpened.Value).ToList();
            if (cityIds != null && cityIds.Any())
                stores = stores.Where(a => cityIds.Contains(a.District.CityId)).ToList();
            if (districtIds != null && districtIds.Any())
                stores = stores.Where(a => districtIds.Contains(a.DistrictId)).ToList();
            if (supplierIds != null && supplierIds.Any())
                stores = stores.Where(a => a.Suppliers.Any(a => supplierIds.Contains(a.Id))).ToList();
            if (productDeliveryIds != null && productDeliveryIds.Any())
                stores = stores.Where(a => a.ProductDeliveries.Any(a => productDeliveryIds.Contains(a.Id))).ToList();
            if (packagingIds != null && packagingIds.Any())
                stores = stores.Where(a => a.ProductForSales.Any(a => packagingIds.Contains(a.PackagingId))).ToList();
            if (productTypeIds != null && productTypeIds.Any())
                stores = stores.Where(a => a.ProductForSales.Any(pfs => productTypeIds.Contains(pfs.Product.ProductTypeId))).ToList();
            if (ownershipTypeIds != null && ownershipTypeIds.Any())
                stores = stores.Where(a => ownershipTypeIds.Contains(a.OwnershipTypeId)).ToList();
            
            return new ObservableCollection<Store>(stores);
        }
        
        
    }
}
