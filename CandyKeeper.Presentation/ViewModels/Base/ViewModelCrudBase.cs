using System.Collections.ObjectModel;
using System.Windows.Input;
using CandyKeeper.Presentation.Infrastructure.Commands;

namespace CandyKeeper.Presentation.ViewModels.Base;

internal class ViewModelCrudBase<T> : ViewModel
                    where T: class
{
    protected ObservableCollection<T> _collection;

    public ObservableCollection<T> Collection
    {
        get => _collection;
        set => Set(ref _collection, value);
    }
    
    #region Команды
    
    public virtual ICommand GetCommand { get; }
    
    protected virtual bool CanGetCommandExecute(object p) => true;

    protected virtual async void OnGetCommandExecuted(object p)
    {

    }

    #endregion

    public ViewModelCrudBase()
    {
        GetCommand = new LambdaCommand(OnGetCommandExecuted);
    }
}