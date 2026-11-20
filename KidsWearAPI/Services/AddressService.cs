using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<AddressResponse> AddAddress(
            int userId, AddressRequest request)
        {
            var address = new Address
            {
                UserId = userId,
                FullName = request.FullName,
                Phone = request.Phone,
                AddressLine = request.AddressLine,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country
            };

            
            bool hasAddress = await _context.Addresses
                .AnyAsync(a => a.UserId == userId);

            if (!hasAddress)
                address.IsDefault = true;

            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();

            return MapAddress(address);
        }

        
        public async Task<IEnumerable<AddressResponse>> GetAddresses(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new AddressResponse
                {
                    AddressId = a.AddressId,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
        }

        
        public async Task<AddressResponse?> UpdateAddress(
            int userId, int addressId, AddressRequest request)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
                return null;

            address.FullName = request.FullName;
            address.Phone = request.Phone;
            address.AddressLine = request.AddressLine;
            address.City = request.City;
            address.State = request.State;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;

            await _context.SaveChangesAsync();

            return MapAddress(address);
        }

       
        public async Task<bool> DeleteAddress(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
                return false;

            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();

            
            if (address.IsDefault)
            {
                var newDefault = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId);

                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        
        public async Task<bool> SetDefaultAddress(int userId, int addressId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var selectedAddress = addresses
                .FirstOrDefault(a => a.AddressId == addressId);

            if (selectedAddress == null)
                return false;

            foreach (var address in addresses)
            {
                address.IsDefault = address.AddressId == addressId;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        private static AddressResponse MapAddress(Address address)
        {
            return new AddressResponse
            {
                AddressId = address.AddressId,
                FullName = address.FullName,
                Phone = address.Phone,
                AddressLine = address.AddressLine,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }
    }
}