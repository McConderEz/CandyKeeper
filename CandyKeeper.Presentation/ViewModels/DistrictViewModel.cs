﻿using CandyKeeper.Application.Interfaces;
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
    internal class DistrictViewModel : ViewModel
    {
        public delegate void Delegate(object p);
        private static event Delegate _refreshEvent;
        private static event Delegate _closeEvent;
        private static event Delegate _returnEvent;
        
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IDistrictService _districtService;
        private readonly ICityService _cityService;
        private readonly IStoreService _storeService;

        private ObservableCollection<District> _districts;
        private ObservableCollection<City> _cities;
        private ObservableCollection<Store> _stores;


        private Models.District _selectedItem = new();
        private District _selectedItemForDetails;

        private DetailsDistrictPage _detailsView;
        
        #region Команды

        #region  GetCommand

        public ICommand GetCommand { get; }
        private bool CanGetCommandExecute(object p) => true;
        public async void OnGetCommandExecuted(object p)
        {
            await _semaphore.WaitAsync();
            try
            {
                Districts = new ObservableCollection<District>(await _districtService.Get());
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
                var page = new AddEditDistrictPage();
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
                    var district = District.Create(
                        _selectedItem.Id,
                        _selectedItem.Name,
                        _selectedItem.CityId);

                    if (district.IsFailure)
                        throw new ArgumentException();

                    await _districtService.Create(district.Value);
                }
                else
                {
                    var district = District.Create(
                        _selectedItem.Id,
                        _selectedItem.Name,
                        _selectedItem.CityId);

                    if (district.IsFailure)
                        throw new ArgumentException();

                    await _districtService.Update(district.Value);
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
                    await _districtService.Delete(id);
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
                    SelectedItemForDetails = await _districtService.GetById(id);
                    _closeEvent?.Invoke(null);
                    DetailsPage = new DetailsDistrictPage();
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

        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => Set(ref _stores, value);
        }
        
        public Models.District SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
        
        public District SelectedItemForDetails
        {
            get => _selectedItemForDetails;
            set => Set(ref _selectedItemForDetails, value);
        }

        public DetailsDistrictPage DetailsPage
        {
            get => _detailsView;
            set => Set(ref _detailsView, value);
        }
        
        public DistrictViewModel(IDistrictService districtService,
                             ICityService cityService, 
                             IStoreService storeService)
        {
            _districtService = districtService;
            _cityService = cityService;
            _storeService = storeService;
            
            GetCommand = new LambdaCommand(OnGetCommandExecuted);
            AddEditShowCommand = new LambdaCommand(OnAddEditShowCommandExecuted);
            AddEditCommand = new LambdaCommand(OnAddEditCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted);
            DetailsCommand = new LambdaCommand(OnDetailsCommandExecuted);
            ReturnCommand = new LambdaCommand(OnReturnCommandExecuted);
            
            _districts = new ObservableCollection<District>();
            OnGetCommandExecuted(null);
        }

        private async Task LoadComboBoxes()
        { 
            await GetStores();
            await GetCities();
        }
        
        private async Task GetStores()
        {
            Stores = new ObservableCollection<Store>(await _storeService.Get());
        }
        
        private async Task GetCities()
        {
            Cities = new ObservableCollection<City>(await _cityService.Get());
        }
    }
}
