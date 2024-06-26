﻿using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL
{
    public interface ISupplierRepository
    {
        Task Create(Supplier supplier);
        Task Delete(int id);
        Task<List<Supplier>> Get();
        Task<Supplier> GetById(int id);
        Task Update(int id, int storeNumber, string name, string phone, int ownershipTypeId, int cityId);
    }
}