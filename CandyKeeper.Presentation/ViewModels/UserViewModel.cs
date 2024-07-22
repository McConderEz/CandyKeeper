using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Presentation.Infrastructure.Commands;
using CandyKeeper.Presentation.Models;
using CandyKeeper.Presentation.Views.Windows;
using MaterialDesignThemes.Wpf;

namespace CandyKeeper.Presentation.ViewModels.Base;

internal class UserViewModel: ViewModel
{



    private static event EventHandler _closeEvent;
    
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private bool _isInvalidCredentials = false;

    private User _currentUser;
    
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

    public ICommand GoToRegCommand { get; }
    private bool CanGoToRegCommandExecute(object p) => true;
    public async void OnGoToRegCommandExecuted(object p)
    {
        await _semaphore.WaitAsync();
        try
        {
            _closeEvent?.Invoke(null,EventArgs.Empty);
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
            _closeEvent?.Invoke(null,EventArgs.Empty);
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

    public bool IsInvalidCredentials
    {
        get => _isInvalidCredentials;
        set => Set(ref _isInvalidCredentials, value);
    }

    public static event EventHandler CloseEvent
    {
        add => _closeEvent += value;
        remove => _closeEvent -= value;
    }
    
    public UserViewModel(IUserService userService, IAccountService accountService)
    {
        _userService = userService;
        _accountService = accountService;
        
        LoginCommand = new LambdaCommand(OnLoginCommandExecuted);
        GoToRegCommand = new LambdaCommand(OnGoToRegCommandExecuted);
        RegistrationCommand = new LambdaCommand(OnRegistrationCommandExecuted);
        GoToLoginCommand = new LambdaCommand(OnGoToLoginCommandExecuted);
        
        CurrentUser = new User();
        
        _accountService.AddRoot();
    }
}