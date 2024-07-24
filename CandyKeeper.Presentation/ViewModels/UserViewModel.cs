using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Models;
using CandyKeeper.Presentation.Views.AddEditPages;
using CandyKeeper.Presentation.Views.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CandyKeeper.Presentation.ViewModels.Base;

internal class UserViewModel: ViewModel
{

    private static event EventHandler<User> _closeEvent;
    private static event EventHandler<User> _showMainEvent;
    private static event EventHandler _refreshEvent;
    
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly IConfiguration _configuration;
    

    private bool _isInvalidCredentials = false;

    private User _currentUser;
    private User _selectedUser = new();
    private int previousPrincipalId;
    private ObservableCollection<Domain.Models.User> _users;
    public List<DatabaseRole> _rolesToComboBox;

    public static List<DatabaseRole> Roles;

    public List<DatabaseRole> RolesToComboBox
    {
        get => _rolesToComboBox;
        set => Set(ref _rolesToComboBox, value);
    }

    public User SelectedUser
    {
        get => _selectedUser;
        set => Set(ref _selectedUser, value);
    }
    
    
    
    public ICommand EditRoleShowCommand { get; }
    private bool CanEditRoleShowCommandExecute(object p) => true;
    public async void OnEditRoleShowCommandExecuted(object p)
    {
        if (p is int id)
        {
            SelectedUser.Id = id;
            previousPrincipalId = SelectedUser.PrincipalId;
        }

        await _semaphore.WaitAsync();

        try
        {
            var page = new EditRolePage();
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
    
    public ICommand EditRoleCommand { get; }
    private bool CanEditRoleCommandExecute(object p) => true;
    public async void OnEditRoleCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            var userEntity = await _userService.GetById(SelectedUser.Id);

            await _userService.Update(
                userEntity.Id,
                userEntity.Name,
                SelectedUser.PrincipalId,
                userEntity.StoreId);
            _accountService.DropRoleToUser(
                _configuration.GetConnectionString("DefaultConnection")!,
                userEntity.Name,
                Roles.SingleOrDefault(r => r.PrincipalId == previousPrincipalId).Name);
             
            _accountService.AssignRoleToUser(
                _configuration.GetConnectionString("DefaultConnection")!,
                userEntity.Name,
                Roles.SingleOrDefault(r => r.PrincipalId == SelectedUser.PrincipalId).Name);
            _refreshEvent?.Invoke(null, EventArgs.Empty);
            
        }
        catch (Exception ex)
        {
            IsInvalidCredentials = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public ICommand LoginCommand { get; }
    private bool CanLoginCommandExecute(object p) => true;
    public async void OnLoginCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            var user = await _accountService.Login(_currentUser.Name, _currentUser.PasswordHashed);

            if (user == null)
                throw new Exception("user null");
            
            CurrentUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                PasswordHashed = user.PasswordHashed,
                PrincipalId = user.PrincipalId,
                StoreId = user.StoreId
            };
            MainWindow window = new MainWindow(CurrentUser);
            _showMainEvent?.Invoke(null, CurrentUser);
            window.Show();
            
        }
        catch (Exception ex)
        {
            IsInvalidCredentials = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    
    public ICommand GetCommand { get; }
    private bool CanGetCommandExecute(object p) => true;
    public async void OnGetCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            Users = new ObservableCollection<Domain.Models.User>(await _userService.Get());
            Users.Remove(Users.SingleOrDefault(x => x.Name == "Root"));
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
    
    public ICommand GoToRegCommand { get; }
    private bool CanGoToRegCommandExecute(object p) => true;
    public async void OnGoToRegCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            _closeEvent?.Invoke(null,null);
            var register = new RegisterWindow();
            register.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    
    public ICommand RegistrationCommand { get; }
    private bool CanRegistrationCommandExecute(object p) => true;
    public async void OnRegistrationCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            await _accountService.Register(_currentUser.Name, _currentUser.PasswordHashed);

            CurrentUser = new User();
        }
        catch (Exception ex)
        {
            IsInvalidCredentials = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public ICommand GoToLoginCommand { get; }
    private bool CanGoToLoginCommandExecute(object p) => true;
    public async void OnGoToLoginCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            _closeEvent?.Invoke(null,null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public User CurrentUser
    {
        get => _currentUser;
        set => Set(ref _currentUser, value);
    }

    public ObservableCollection<Domain.Models.User> Users
    {
        get => _users;
        set => Set(ref _users, value);
    }
    
    public bool IsInvalidCredentials
    {
        get => _isInvalidCredentials;
        set => Set(ref _isInvalidCredentials, value);
    }

    public static event EventHandler<User> CloseEvent
    {
        add => _closeEvent += value;
        remove => _closeEvent -= value;
    }
    
    public static event EventHandler<User> ShowMainEvent
    {
        add => _showMainEvent += value;
        remove => _showMainEvent -= value;
    }
    
    public static event EventHandler RefreshEvent
    {
        add => _refreshEvent += value;
        remove => _refreshEvent -= value;
    }
    
    
    public UserViewModel(IUserService userService, IAccountService accountService, IConfiguration configuration)
    {
        _userService = userService;
        _accountService = accountService;
        
        LoginCommand = new LambdaCommand(OnLoginCommandExecuted);
        GoToRegCommand = new LambdaCommand(OnGoToRegCommandExecuted);
        RegistrationCommand = new LambdaCommand(OnRegistrationCommandExecuted);
        GoToLoginCommand = new LambdaCommand(OnGoToLoginCommandExecuted);
        EditRoleShowCommand = new LambdaCommand(OnEditRoleShowCommandExecuted);
        EditRoleCommand = new LambdaCommand(OnEditRoleCommandExecuted);
        
        _configuration = configuration;
        CurrentUser = new User();
        Roles = GetDatabaseRoles();
        RolesToComboBox = Roles;
        OnGetCommandExecuted(null);
        _accountService.AddRoot();

        MainWindowsViewModel.TransferCurrentUserEvent += GetCurrentUser;
    }

    private void GetCurrentUser(object? sender, User e)
    {
        CurrentUser = e;
    }

    private List<DatabaseRole> GetDatabaseRoles()
    {
        string query = "SELECT principal_id, name FROM sys.database_principals WHERE type = 'R'";
        var roles = new List<DatabaseRole>();
        var temp = new string[]{"Admin", "Client", "Manager"};
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (temp.Any(t => t == reader.GetString(1)))
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
        }

        return roles;
    }
    
    public class DatabaseRole
    {
        public int PrincipalId { get; set; }
        public string Name { get; set; }
    }
    
}