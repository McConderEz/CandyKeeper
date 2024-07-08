﻿using CandyKeeper.Application.Interfaces;
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
    internal class OwnershipTypeViewModel: ViewModel
    {
        private readonly IOwnershipTypeService _service;

        private ObservableCollection<OwnershipType> _ownershipTypes;

        public ObservableCollection<OwnershipType> OwnershipTypes
        {
            get => _ownershipTypes;
            set => Set(ref _ownershipTypes, value);
        }

        public OwnershipTypeViewModel(IOwnershipTypeService service)
        {
            _service = service;
        }
    }
}