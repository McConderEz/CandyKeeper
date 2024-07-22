using CandyKeeper.Application.Interfaces;

namespace CandyKeeper.Presentation.ViewModels.Base;

internal class UserViewModel: ViewModel
{
    private readonly IUserService _service;

    public UserViewModel(IUserService service)
    {
        _service = service;
    }
}