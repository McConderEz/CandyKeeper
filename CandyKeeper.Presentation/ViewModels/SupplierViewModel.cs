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
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class SupplierViewModel: ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly IProductDeliveryService _productDeliveryService;
        private readonly IStoreService _storeService;
        private readonly ISupplierService _service;
        private readonly IProductForSaleService _productForSaleService;
        private readonly IOwnershipTypeService _ownershipTypeService;
        private readonly ICityService _cityService;
        
        private ObservableCollection<ProductDelivery> _productDeliveries;
        
        private ObservableCollection<Store> _stores;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<ProductForSale> _productForSales;
        private ObservableCollection<OwnershipType> _ownershipTypes;
        private ObservableCollection<City> _cities;

        private Models.Supplier _selectedItem = new();
        private Supplier _selectedItemForDetails;

        private int _storeId;

        private ComboBoxItem _selectedDataGridInDetailsPage;
        
        private DetailsSupplierPage _detailsView;
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
                Suppliers = new ObservableCollection<Supplier>(await _service.Get());
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
                var page = new AddEditSupplierPage();
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
                    var supplier = Supplier.Create(
                        _selectedItem.Id,
                        _selectedItem.Name,
                        _selectedItem.OwnershipTypeId,
                        _selectedItem.CityId,
                        _selectedItem.Phone
                    );

                    if (supplier.IsFailure)
                        throw new ArgumentException();

                    await _service.Create(supplier.Value);
                }
                else
                {
                    var supplier = Supplier.Create(
                        _selectedItem.Id,
                        _selectedItem.Name,
                        _selectedItem.OwnershipTypeId,
                        _selectedItem.CityId,
                        _selectedItem.Phone
                    );

                    if (supplier.IsFailure)
                        throw new ArgumentException();

                    await _service.Update(supplier.Value);
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
                    DetailsPage = new DetailsSupplierPage();
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
        
        public ICommand SearchCommand { get; }
        private bool CanSearchCommandExecute(object p) => true;
        public async void OnSearchCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(SearchingString))
                {
                    Suppliers = new ObservableCollection<Supplier>(await _service.GetBySearchingString(SearchingString));
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
        
        #region AddStoreInSupplierShowCommand
        public ICommand AddStoreInSupplierShowCommand { get; }
        private bool CanAddStoreInSupplierShowCommandExecute(object p) => true;
        public async void OnAddStoreInSupplierShowCommandExecuted(object p)
        {
            if (p is int id)
            {
                SelectedItem.Id = id;
            }

            await _semaphore.WaitAsync();

            try
            {
                await GetStores();
                var page = new AddStoreInSupplierPage();
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
        public ICommand AddStoreInSupplierCommand { get; }
        private bool CanAddSupplierInStoreCommandExecute(object p) => true;
        public async void OnAddStoreInSupplierCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    var store = await _storeService.GetById(_storeId);
                    await _service.AddStore(_selectedItemForDetails.Id, store);
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
        
        public ICommand DeleteStoreFromSupplierCommand { get; }
        private bool CanDeleteStoreFromSupplierCommandExecuted(object p) => true;
        public async void OnDeleteStoreFromSupplierCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (p is int id)
                {
                    var store = await _storeService.GetById(id);
                    await _service.DeleteStore(_selectedItemForDetails.Id, store);
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
        
        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
        }
        
        public Models.Supplier SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public Supplier SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsSupplierPage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }

        public int StoreId
        {
            get => _storeId;
            set => Set(ref _storeId, value);
        }

        public ComboBoxItem SelectedDataGridInDetailsPage
        {
            get => _selectedDataGridInDetailsPage;
            set => Set(ref _selectedDataGridInDetailsPage, value);
        }
        

        public SupplierViewModel(IProductDeliveryService productDeliveryService,
            IStoreService storeService, 
            ISupplierService service,
            IProductForSaleService productForSaleService,
            IOwnershipTypeService ownershipTypeService,
            ICityService cityService)
        {
            _service = service;
            _productDeliveryService = productDeliveryService;
            _storeService = storeService;
            _productForSaleService = productForSaleService;
            _ownershipTypeService = ownershipTypeService;
            _cityService = cityService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            AddStoreInSupplierShowCommand = new LambdaCommand(OnAddStoreInSupplierShowCommandExecuted);
            AddStoreInSupplierCommand = new LambdaCommand(OnAddStoreInSupplierCommandExecuted);
            DeleteStoreFromSupplierCommand = new LambdaCommand(OnDeleteStoreFromSupplierCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            
            _productDeliveries = new ObservableCollection<ProductDelivery>();
            OnGetCommandExecuted(null);
        }
        
        private async Task LoadComboBoxes()
        { 
            await GetSuppliers();
            await GetOwnershipTypes();
            await GetCities();
            await GetProductForSales();
        }
        
        
        private async Task GetStores()
        {
            Stores = new ObservableCollection<Store>(await _storeService.Get());
        }
        
        
        private async Task GetSuppliers()
        {
            Suppliers = new ObservableCollection<Supplier>(await _service.Get());
        }

        private async Task GetProductForSales()
        {
            ProductForSales = new ObservableCollection<ProductForSale>(await _productForSaleService.Get());
        }

        private async Task GetOwnershipTypes()
        {
            OwnershipTypes = new ObservableCollection<OwnershipType>(await _ownershipTypeService.Get());
        }
        
        private async Task GetCities()
        {
            Cities = new ObservableCollection<City>(await _cityService.Get());
        }
    }
}
