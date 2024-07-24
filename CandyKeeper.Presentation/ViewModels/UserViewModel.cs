using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Models;
using CandyKeeper.Presentation.Views.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CandyKeeper.Presentation.ViewModels.Base;

internal class UserViewModel: ViewModel
{

    private static event EventHandler<User> _closeEvent;
    private static event EventHandler<User> _showMainEvent;
    
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly IConfiguration _configuration;
    

    private bool _isInvalidCredentials = false;

    private User _currentUser;
    private ObservableCollection<Domain.Models.User> _users;
    public static List<DatabaseRole> Roles;
    
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
    
    public UserViewModel(IUserService userService, IAccountService accountService, IConfiguration configuration)
    {
        _userService = userService;
        _accountService = accountService;
        
        LoginCommand = new LambdaCommand(OnLoginCommandExecuted);
        GoToRegCommand = new LambdaCommand(OnGoToRegCommandExecuted);
        RegistrationCommand = new LambdaCommand(OnRegistrationCommandExecuted);
        GoToLoginCommand = new LambdaCommand(OnGoToLoginCommandExecuted);

        _configuration = configuration;
        CurrentUser = new User();
        Roles = GetDatabaseRoles();
        OnGetCommandExecuted(null);
        _accountService.AddRoot();
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
    
    public class DatabaseRole
    {
        public int PrincipalId { get; set; }
        public string Name { get; set; }
    }
    
}