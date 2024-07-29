using CandyKeeper.Application.Interfaces;
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
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.DetailsPages;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class PackagingViewModel: ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private readonly IPackagingService _packagingService;
        private readonly IProductForSaleService _productForSaleService;

        
        private ObservableCollection<Packaging> _packagings;
        
        private ObservableCollection<ProductForSale> _productForSales;


        private Models.Packaging _selectedItem = new();
        private Packaging _selectedItemForDetails;

        private DetailsPackagingPage _detailsView;
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
                Packagings = new ObservableCollection<Packaging>(await _packagingService.Get());
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
                var page = new AddEditPackagingPage();
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
                    var packaging = Packaging.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);

                    if (packaging.IsFailure)
                        throw new ArgumentException();

                    await _packagingService.Create(packaging.Value);
                }
                else
                {
                    var packaging = Packaging.Create(
                        _selectedItem.Id,
                        _selectedItem.Name);

                    if (packaging.IsFailure)
                        throw new ArgumentException();

                    await _packagingService.Update(packaging.Value);
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
                    await _packagingService.Delete(id);
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
                    SelectedItemForDetails = await _packagingService.GetById(id);
                    _closeEvent?.Invoke(null);
                    DetailsPage = new DetailsPackagingPage();
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
                    Packagings = new ObservableCollection<Packaging>(await _packagingService.GetBySearchingString(SearchingString));
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
        
        public ObservableCollection<Packaging> Packagings
        {
            get => _packagings;
            set => Set(ref _packagings, value);
        }

        public ObservableCollection<ProductForSale> ProductForSales
        {
            get => _productForSales;
            set => Set(ref _productForSales, value);
        }
        
        public Models.Packaging SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public Packaging SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsPackagingPage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }
        
        public PackagingViewModel(IPackagingService packagingService, 
                             IProductForSaleService productForSaleService)
        {
            _packagingService = packagingService;
            _productForSaleService = productForSaleService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            SearchCommand = new LambdaCommand(OnSearchCommandExecuted);
            
            _packagings = new ObservableCollection<Packaging>();
            OnGetCommandExecuted(null);
        }

        private async Task LoadComboBoxes()
        { 
            await GetProductForSales();
        }
        
        private async Task GetProductForSales()
        {
            ProductForSales = new ObservableCollection<ProductForSale>(await _productForSaleService.Get());
        }
    }
}
