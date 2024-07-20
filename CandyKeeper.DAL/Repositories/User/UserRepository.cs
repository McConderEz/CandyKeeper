﻿using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CandyKeeper.DAL.Repositories.User;

public class UserRepository : IUserRepository
{
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public UserRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Models.User>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var userEntities = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Store)
                    .ToListAsync();

                var users = userEntities.Select(s => MapToUser(s)).ToList();


                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private Domain.Models.User MapToUser(UserEntity userEntity)
        {
            var store = userEntity.Store != null ? MapToStore(userEntity.Store) : null;
            
            return Domain.Models.User.Create(
                userEntity.Id,
                userEntity.Name,
                userEntity.PasswordHashed,
                userEntity.StoreId,
                store).Value;
        }

        private Store MapToStore(StoreEntity userEntityStore)
        {
            return Store.Create(
                userEntityStore.Id,
                userEntityStore.StoreNumber,
                userEntityStore.Name,
                userEntityStore.YearOfOpened,
                userEntityStore.Phone,
                userEntityStore.OwnershipTypeId,
                userEntityStore.DistrictId).Value;
        }

        public async Task<Domain.Models.User> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var userEntity = await _context.Users
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (userEntity == null)
                    throw new Exception("supplier null");


                var user = MapToUser(userEntity);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Create(Domain.Models.User user)
        {
            await _semaphore.WaitAsync();

            try
            {
                var userEntity = MapToUserEntity(user);

                await _context.Users.AddAsync(userEntity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private UserEntity MapToUserEntity(Domain.Models.User user)
        {
            return new UserEntity
            {
                Id = user.Id,
                Name = user.Name,
                PasswordHashed = user.PasswordHashed,
                StoreId = user.StoreId
            };
        }

        public async Task Update(int id, string name, int? storeId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Users
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Name, name)
                        .SetProperty(p => p.StoreId, storeId));

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Delete(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Users
                    .Where(p => p.Id == id)
                    .ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
}