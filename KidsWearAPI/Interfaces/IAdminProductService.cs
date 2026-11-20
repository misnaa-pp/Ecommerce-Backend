
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAdminProductService
    {


        Task<PaginatedResponse<ProductResponse>> GetAllProducts(int pageNumber,int pageSize, string? search,string? category);
        Task<ProductResponse?> GetProductById(int id);
        Task<ProductResponse> AddProduct(ProductRequest request);
        Task<ProductResponse?> UpdateProduct(int id, ProductRequest request);
        Task<string> DeleteProduct(int id);

    }
}
