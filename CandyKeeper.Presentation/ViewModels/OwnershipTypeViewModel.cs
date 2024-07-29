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
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class OwnershipTypeViewModel: ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IOwnershipTypeService _ownershipTypeService;
        private readonly ISupplierService _supplierService;
        private readonly IStoreService _storeService;

        private ObservableCollection<OwnershipType> _ownershipTypes;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<Store> _stores;


        private Models.OwnershipType _selectedItem = new();
        private OwnershipType _selectedItemForDetails;

        private DetailsOwnershipTypePage _detailsView;
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
                OwnershipTypes = new ObservableCollection<OwnershipType>(await _ownershipTypeService.Get());
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
                var page = new AddEditOwnershipTypePage();
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
                    var ownershipType = OwnershipType.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);

                    if (ownershipType.IsFailure)
                        throw new ArgumentException();

                    await _ownershipTypeService.Create(ownershipType.Value);
                }
                else
                {
                    var ownershipType = OwnershipType.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);

                    if (ownershipType.IsFailure)
                        throw new ArgumentException();

                    await _ownershipTypeService.Update(ownershipType.Value);
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
                    await _ownershipTypeService.Delete(id);
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
                    SelectedItemForDetails = await _ownershipTypeService.GetById(id);
                    _closeEvent?.Invoke(null);
                    DetailsPage = new DetailsOwnershipTypePage();
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
                    OwnershipTypes = new ObservableCollection<OwnershipType>(await _ownershipTypeService.GetBySearchingString(SearchingString));
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
        
        public ObservableCollection<OwnershipType> OwnershipTypes
        {
            get => _ownershipTypes;
            set => Set(ref _ownershipTypes, value);
        }
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
        }

        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => Set(ref _stores, value);
        }
        
        public Models.OwnershipType SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public OwnershipType SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsOwnershipTypePage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }
        
        public OwnershipTypeViewModel(IOwnershipTypeService ownershipTypeService,
                             ISupplierService supplierService, 
                             IStoreService storeService)
        {
            _ownershipTypeService = ownershipTypeService;
            _supplierService = supplierService;
            _storeService = storeService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            
            _ownershipTypes = new ObservableCollection<OwnershipType>();
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
    }
}
