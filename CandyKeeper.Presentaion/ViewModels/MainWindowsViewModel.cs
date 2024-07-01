using CandyKeeper.Presentaion.Infrastructure.Commands;
using CandyKeeper.Presentaion.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace CandyKeeper.Presentaion.ViewModels
{
    internal class MainWindowsViewModel : ViewModel
    {
        private string _title = "CandyKeeper";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private void OnCloseApplicationCommandExecute(object p)
        {
            Application.Current.Shutdown();
        }

        private bool CanCloseApplicationCommandExecute(object p) => true;
        #endregion

        #endregion

        public MainWindowsViewModel()
        {
            #region Команды
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecute);

            #endregion
        }
    }
}
