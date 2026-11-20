
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IProductService
    {

        Task<PagedResponse<ProductResponse>> GetAllProducts(
            string? search,
            string? category,
            int pageNumber,
            int pageSize );


        Task<ProductResponse?> GetProductById(int id);
    }
}
