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
    internal class StoreViewModel : ViewModel
    {
        private readonly IStoreService _service;

        private ObservableCollection<Store> _stores;

        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => Set(ref _stores, value);
        }

        public StoreViewModel(IStoreService service)
        {
            _service = service;
        }
    }
}