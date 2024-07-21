using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyKeeper.Presentation.ViewModels.Base;

namespace CandyKeeper.Presentation.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowsViewModel MainWindowsViewModel => App.Host.Services.GetRequiredService<MainWindowsViewModel>();
        public CityViewModel CityViewModel => App.Host.Services.GetRequiredService<CityViewModel>();
        public DistrictViewModel DistrictViewModel => App.Host.Services.GetRequiredService<DistrictViewModel>();
        public OwnershipTypeViewModel OwnershipTypeViewModel => App.Host.Services.GetRequiredService<OwnershipTypeViewModel>();
        public PackagingViewModel PackagingViewModel => App.Host.Services.GetRequiredService<PackagingViewModel>();
        public ProductDeliveryViewModel ProductDeliveryViewModel => App.Host.Services.GetRequiredService<ProductDeliveryViewModel>();
        public ProductForSaleViewModel ProductForSaleViewModel => App.Host.Services.GetRequiredService<ProductForSaleViewModel>();
        public ProductTypeViewModel ProductTypeViewModel => App.Host.Services.GetRequiredService<ProductTypeViewModel>();
        public ProductViewModel ProductViewModel => App.Host.Services.GetRequiredService<ProductViewModel>();
        public StoreViewModel StoreViewModel => App.Host.Services.GetRequiredService<StoreViewModel>();
        public SupplierViewModel SupplierViewModel => App.Host.Services.GetRequiredService<SupplierViewModel>();
        //public SupplierViewModel UserViewModel => App.Host.Services.GetRequiredService<UserViewModel>();
    }
}
