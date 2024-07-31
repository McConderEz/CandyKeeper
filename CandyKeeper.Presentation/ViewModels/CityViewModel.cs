using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;
using CSharpFunctionalExtensions;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class CityViewModel : ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly ICityService _cityService;
        private readonly ISupplierService _supplierService;
        private ObservableCollection<City> _cities;
        private ObservableCollection<Supplier> _suppliers;
        private Models.City _selectedItem = new();
        private City _selectedItemForDetails;
        private DetailsCityPage _detailsView;
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
                if (p == null)
                {
                    Cities = new ObservableCollection<City>(await _cityService.Get());
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
                var page = new AddEditCityPage();
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
                    var city = City.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);

                    if (city.IsFailure)
                        throw new ArgumentException();
                    
                    await _cityService.Create(city.Value);
                }
                else
                {
                    var city = City.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);
                    
                    if (city.IsFailure)
                        throw new ArgumentException();
                    
                    await _cityService.Update(city.Value);
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
                    await _cityService.Delete(id);
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
                    SelectedItemForDetails = await _cityService.GetById(id);
                    _closeEvent?.Invoke(null);
                    DetailsPage = new DetailsCityPage();
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

        #region SearchCommand

        public ICommand SearchCommand { get; }
        private bool CanSearchCommandExecute(object p) => true;
        public async void OnSearchCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(SearchingString))
                {
                    Cities = new ObservableCollection<City>(await _cityService.GetBySearchingString(SearchingString));
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
                var page = new FilterCityPage();
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

                var cities = await _cityService.Get();
                
                Cities = Filter(MinStoreCount, MaxStoreCount, MinSupplierCount, MaxSupplierCount, MinDistrictCount,
                    MaxDistrictCount, cities);

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
        
        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => Set(ref _cities, value);
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
        }
        
        public Models.City SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public City SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsCityPage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }
        
        public CityViewModel(ICityService cityService, 
                             ISupplierService supplierService)
        {
            _cityService = cityService;
            _supplierService = supplierService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            FilterShowCommand = new LambdaCommand(OnFilterShowCommandExecuted);
            FilterCommand = new LambdaCommand(OnFilterCommandExecuted);
            
            _cities = new ObservableCollection<City>();
            OnGetCommandExecuted(null);
        }

        private async Task LoadComboBoxes()
        { 
            await GetSuppliers();
        }
        
        private async Task GetSuppliers()
        {
            Suppliers = new ObservableCollection<Supplier>(await _supplierService.Get());
        }
        
        #region Поля_фильтрации

        private int? _minStoreCount;
        private int? _maxStoreCount;

        private int? _minSupplierCount;
        private int? _maxSupplierCount;

        private int? _minDistrictCount;
        private int? _maxDistrictCount;

        public int? MinStoreCount
        {
            get => _minStoreCount;
            set => Set(ref _minStoreCount, value);
        }
        
        public int? MaxStoreCount
        {
            get => _maxStoreCount;
            set => Set(ref _maxStoreCount, value);
        }
        
        public int? MinSupplierCount
        {
            get => _minSupplierCount;
            set => Set(ref _minSupplierCount, value);
        }
        
        public int? MaxSupplierCount
        {
            get => _maxSupplierCount;
            set => Set(ref _maxSupplierCount, value);
        }
        
        public int? MinDistrictCount
        {
            get => _minDistrictCount;
            set => Set(ref _minDistrictCount, value);
        }

        public int? MaxDistrictCount
        {
            get => _maxDistrictCount;
            set => Set(ref _maxDistrictCount, value);
        }

        
        #endregion
        
        private static ObservableCollection<City>? Filter(int? minStoreCount, 
            int? maxStoreCount, 
            int? minSupplierCount, 
            int? maxSupplierCount, 
            int? minDistrictCount, 
            int? maxDistrictCount, 
            List<City> cities)
        {
            if (minStoreCount.HasValue)
                cities = cities.Where(a => a.Districts.Sum(d => d.Stores.Count()) >= minStoreCount.Value).ToList();
            if (maxStoreCount.HasValue)
                cities = cities.Where(a => a.Districts.Sum(d => d.Stores.Count()) <= maxStoreCount.Value).ToList();
            if (minSupplierCount.HasValue)
                cities = cities.Where(a => a.Suppliers.Count >= minSupplierCount.Value).ToList();
            if (maxSupplierCount.HasValue)
                cities = cities.Where(a => a.Suppliers.Count <= maxSupplierCount.Value).ToList();
            if (minDistrictCount.HasValue)
                cities = cities.Where(a => a.Districts.Count() >= minDistrictCount.Value).ToList();
            if (maxDistrictCount.HasValue)
                cities = cities.Where(a => a.Districts.Count() <= maxDistrictCount.Value).ToList();
            return new ObservableCollection<City>(cities);
        }
    }
}
