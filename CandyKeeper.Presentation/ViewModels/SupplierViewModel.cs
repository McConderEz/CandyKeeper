using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using CandyKeeper.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class SupplierViewModel: ViewModel
    {
        private readonly ISupplierService _service;

        private ObservableCollection<Supplier> _suppliers;

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
        }

        public SupplierViewModel(ISupplierService service)
        {
            _service = service;
        }
    }
}
