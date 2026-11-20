using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAddressService
    {
        Task<AddressResponse> AddAddress(int userId, AddressRequest request);

        Task<IEnumerable<AddressResponse>> GetAddresses(int userId);

        Task<AddressResponse?> UpdateAddress(
            int userId, int addressId, AddressRequest request);

        Task<bool> DeleteAddress(int userId, int addressId);

        Task<bool> SetDefaultAddress(int userId, int addressId);
    }
}
