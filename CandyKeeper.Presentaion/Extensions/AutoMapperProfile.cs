using AutoMapper;
using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Extensions
{
    public class AutoMapperProfile: Profile
    {
        //TODO: Изменить, чтобы работало с фабричным методом
        public AutoMapperProfile()
        {
            CreateMap<CityEntity, City>()
                .ConvertUsing((src,dest, context) =>
                {
                    var districts = context.Mapper.Map<IEnumerable<District>>(src.Districts);
                    var suppliers = context.Mapper.Map<IEnumerable<Supplier>>(src.Suppliers);

                    var result = City.Create(src.Id, src.Name, districts, suppliers);

                    if (result.IsSuccess)
                    {
                        return result.Value;
                    }
                    else
                    {
                        throw new AutoMapperMappingException(result.Error);
                    }
                });

            CreateMap<ProductEntity, Product>().ReverseMap();
            CreateMap<ProductTypeEntity, ProductType>().ReverseMap();
            CreateMap<DistrictEntity, District>().ReverseMap();
            CreateMap<CityEntity, City>().ReverseMap();
            CreateMap<ProductForSaleEntity, ProductForSale>().ReverseMap();
            CreateMap<StoreEntity, Store>().ReverseMap();
            CreateMap<SupplierEntity, Supplier>().ReverseMap();
            CreateMap<OwnershipTypeEntity, OwnershipType>().ReverseMap();
            CreateMap<ProductDeliveryEntity, ProductDelivery>().ReverseMap();
            CreateMap<PackagingEntity, Packaging>().ReverseMap();
        }
    }
}
