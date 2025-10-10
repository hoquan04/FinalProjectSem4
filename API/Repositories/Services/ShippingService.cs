using API.Models;
using API.Models.DTOs;
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
            => await _shippingRepository.GetAllAsync();

        public async Task<APIRespone<Shipping>> GetShippingByIdAsync(int id)
            => await _shippingRepository.GetByIdAsync(id);

        public async Task<APIRespone<Shipping>> CreateShippingAsync(Shipping shipping)
        {
            if (string.IsNullOrWhiteSpace(shipping.RecipientName) || string.IsNullOrWhiteSpace(shipping.PhoneNumber))
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = "Người nhận và số điện thoại là bắt buộc",
                    Data = null
                };
            }
            return await _shippingRepository.AddAsync(shipping);
        }

        public async Task<APIRespone<Shipping>> UpdateShippingAsync(int id, Shipping shipping)
        {
            if (string.IsNullOrWhiteSpace(shipping.RecipientName) || string.IsNullOrWhiteSpace(shipping.PhoneNumber))
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = "Người nhận và số điện thoại là bắt buộc",
                    Data = null
                };
            }
            return await _shippingRepository.UpdateAsync(id, shipping);
        }

        public async Task<APIRespone<bool>> DeleteShippingAsync(int id)
            => await _shippingRepository.DeleteAsync(id);

        public async Task<APIRespone<PagedResponse<Shipping>>> GetPagedShippingsAsync(int pageNow, int pageSize)
        {
            if (pageNow < 1) pageNow = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return await _shippingRepository.GetPageAsync(pageNow, pageSize);
        }


        // 🔍 Search có phân trang
        public async Task<APIRespone<IEnumerable<Shipping>>> SearchShippingsAsync(ShippingSearchDto dto)
        {
            return await _shippingRepository.SearchAsync(
                dto.RecipientName,
                dto.PhoneNumber,
                dto.Address,
                dto.City,
                dto.PostalCode
            );
        }

        public async Task<APIRespone<PagedResponse<Shipping>>> SearchShippingsWithPaginationAsync(ShippingSearchDto dto)
        {
            if (dto.PageNow < 1) dto.PageNow = 1;
            if (dto.PageSize < 1) dto.PageSize = 10;
            if (dto.PageSize > 100) dto.PageSize = 100;

            return await _shippingRepository.SearchWithPaginationAsync(
                dto.RecipientName,
                dto.PhoneNumber,
                dto.Address,
                dto.City,
                dto.PostalCode,
                dto.PageNow,
                dto.PageSize
            );
        }

    }
}
