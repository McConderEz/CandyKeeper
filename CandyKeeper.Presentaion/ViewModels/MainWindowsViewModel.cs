using CandyKeeper.Presentaion.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
