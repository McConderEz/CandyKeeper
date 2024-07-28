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
        
        private ObservableCollection<ProductDelivery> _productDeliveries;
        
        private ObservableCollection<Store> _stores;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<ProductForSale> _productForSales;
        private ObservableCollection<OwnershipType> _ownershipTypes;
        private ObservableCollection<District> _districts;

        private Models.Store _selectedItem = new();
        private Store _selectedItemForDetails;

        private int _supplierId;

        private ComboBoxItem _selectedDataGridInDetailsPage;
        
        private DetailsStorePage _detailsView;
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
        
        private bool _isInvalid = false;

        public bool IsInvalid
        {
            get => _isInvalid;
            set => Set(ref _isInvalid, value);
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
            IDistrictService districtService)
        {
            _service = service;
            _productDeliveryService = productDeliveryService;
            _supplierService = supplierService;
            _productForSaleService = productForSaleService;
            _ownershipTypeService = ownershipTypeService;
            _districtService = districtService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            AddSupplierInStoreShowCommand = new LambdaCommand(OnAddSupplierInStoreShowCommandExecuted);
            AddSupplierInStoreCommand = new LambdaCommand(OnAddSupplierInStoreCommandExecuted);
            DeleteSupplierFromStoreCommand = new LambdaCommand(OnDeleteSupplierFromStoreCommadExecuted);
            
            _productDeliveries = new ObservableCollection<ProductDelivery>();
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

        
    }
}
