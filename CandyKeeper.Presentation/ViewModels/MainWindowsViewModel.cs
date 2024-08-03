using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.Views.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User = CandyKeeper.Presentation.Models.User;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class MainWindowsViewModel : ViewModel
    {
        private static event EventHandler<User> _transferCurrentUserEvent;
        private static event EventHandler<User> _leaveAccountEvent;
        
        private UserControl _currentView;
        private ViewModelLocator _viewModelLocator;
        private User _currentUser;

        private readonly List<DatabaseRole> _roles;
        private readonly IConfiguration _configuration;

        
        public bool IsAdminVisible => IsAdmin();
        public bool IsClientVisible => IsClient();
        public bool IsManagerVisible => IsManager();
       
        public UserControl CurrentView
        {
            get => _currentView;
            set => Set(ref _currentView, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => Set(ref _currentUser, value);
        }
        
        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private void OnCloseApplicationCommandExecute(object p)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanCloseApplicationCommandExecute(object p) => true;
        #endregion

        #region SelectViewModelCommand
        public ICommand SelectViewCommand { get; }
        private void OnSelectViewCommandExecute(object p)
        {
            if (p is string viewmodel)
            {
                CurrentView = SelectViewModel(viewmodel);
                _transferCurrentUserEvent?.Invoke(null,CurrentUser);
            }
            else
            {
                throw new ArgumentNullException("p is null!");
            }
        }
        private bool CanSelectViewCommandExecute(object p) => true;
        #endregion
        
        public ICommand LeaveAccountCommand { get; }

        private void OnLeaveAccountCommandExecute(object p)
        {
            _leaveAccountEvent?.Invoke(null,null);
        }

        private bool CanLeaveAccountCommandExecute(object p) => true;
        #endregion


        public static event EventHandler<User> TransferCurrentUserEvent
        {
            add => _transferCurrentUserEvent += value;
            remove => _transferCurrentUserEvent -= value;
        }
        
        public static event EventHandler<User> LeaveAccountEvent
        {
            add => _leaveAccountEvent += value;
            remove => _leaveAccountEvent -= value;
        } 
        
        public MainWindowsViewModel(IConfiguration configuration)
        {
            _viewModelLocator = new ViewModelLocator();
            #region Команды
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecute);
            SelectViewCommand = new LambdaCommand(OnSelectViewCommandExecute);
            LeaveAccountCommand = new LambdaCommand(OnLeaveAccountCommandExecute);
            #endregion

            _configuration = configuration;
            _roles = GetDatabaseRoles();
            UserViewModel.ShowMainEvent += GetCurrentUser;
        }

        private void GetCurrentUser(object? sender, User e)
        {
            CurrentUser = e;
            OnPropertyChanged(nameof(IsAdminVisible));
            OnPropertyChanged(nameof(IsClientVisible));
            OnPropertyChanged(nameof(IsManagerVisible));
        }

        public UserControl SelectViewModel(string viewModelName) => viewModelName switch
        {
            "CityView" =>  new CityWindow(),
            "DistrictView" =>  new DistrictWindow(),
            "OwnershipTypeView" =>  new OwnershipTypeWindow(),
            "PackagingView" =>  new PackagingWindow(),
            "ProductDeliveryView" => new ProductDeliveryWindow(),
            "ProductForSaleView" => new ProductForSaleWindow(),
            "ProductTypeView" => new ProductTypeWindow(),
            "ProductView" => new ProductWindow(),
            "StoreView" => new StoreWindow(),
            "SupplierView" => new SupplierWindow(),
            "AdminPanelView" => new AdminPanelWindow(),
            "QueryView" => new QueryWindow(),
            "DiagramView" => new DiagramWindow(),
            _ => throw new ArgumentException("selected view model does not exist exist")
        };

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
        
    }
}
