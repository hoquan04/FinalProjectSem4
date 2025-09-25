using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;

namespace API.Repositories.Services
{
    public class ShippingService
    {
        private readonly IShippingRepository _shippingRepository;

        public ShippingService(IShippingRepository shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task<APIRespone<IEnumerable<Shipping>>> GetAllShippingsAsync()
        {
            return await _shippingRepository.GetAllAsync();
        }

        public async Task<APIRespone<Shipping>> GetShippingByIdAsync(int id)
        {
            return await _shippingRepository.GetByIdAsync(id);
        }

        public async Task<APIRespone<Shipping>> CreateShippingAsync(Shipping shipping)
        {
            if (string.IsNullOrWhiteSpace(shipping.Address))
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = "Địa chỉ giao hàng không được để trống",
                    Data = null
                };
            }

            return await _shippingRepository.AddAsync(shipping);
        }

        public async Task<APIRespone<Shipping>> UpdateShippingAsync(int id, Shipping shipping)
        {
            if (string.IsNullOrWhiteSpace(shipping.Address))
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = "Địa chỉ giao hàng không được để trống",
                    Data = null
                };
            }

            return await _shippingRepository.UpdateAsync(id, shipping);
        }

        public async Task<APIRespone<bool>> DeleteShippingAsync(int id)
        {
            return await _shippingRepository.DeleteAsync(id);
        }

        public async Task<APIRespone<PagedResponse<Shipping>>> GetPagedShippingsAsync(int pageNow, int pageSize)
        {
            if (pageNow < 1) pageNow = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa

            return await _shippingRepository.GetPageAsync(pageNow, pageSize);
        }

        public async Task<APIRespone<IEnumerable<Shipping>>> SearchShippingsAsync(string? address, string? city)
        {
            return await _shippingRepository.SearchAsync(address, city);
        }

        public async Task<APIRespone<PagedResponse<Shipping>>> SearchShippingsWithPaginationAsync(
            string? address, string? city, int pageNow, int pageSize)
        {
            if (pageNow < 1) pageNow = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return await _shippingRepository.SearchWithPaginationAsync(address, city, pageNow, pageSize);
        }
    }
}
