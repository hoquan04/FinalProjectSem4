using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        public CartRepository(DataContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng theo UserId
        public async Task<APIRespone<IEnumerable<CartDto>>> GetCartByUserAsync(int userId)
        {
            var data = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Select(c => new CartDto
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ImageUrl = c.Product.ImageUrl,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                }).ToListAsync();

            return new APIRespone<IEnumerable<CartDto>>
            {
                Success = true,
                Message = "Lấy giỏ hàng thành công",
                Data = data
            };
        }

        // Thêm vào giỏ hàng
        public async Task<APIRespone<CartDto>> AddToCartAsync(int userId, int productId, int quantity)
        {
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                _context.Carts.Update(existing);
            }
            else
            {
                var newItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.Carts.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return await GetCartItemDto(userId, productId);
        }

        // Cập nhật số lượng
        public async Task<APIRespone<CartDto>> UpdateQuantityAsync(int cartId, int quantity)
        {
            var item = await _context.Carts.Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
            if (item == null)
            {
                return new APIRespone<CartDto>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm trong giỏ",
                    Data = null
                };
            }

            item.Quantity = quantity;
            _context.Carts.Update(item);
            await _context.SaveChangesAsync();

            return new APIRespone<CartDto>
            {
                Success = true,
                Message = "Cập nhật số lượng thành công",
                Data = new CartDto
                {
                    CartId = item.CartId,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ImageUrl = item.Product.ImageUrl,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                }
            };
        }

        // Xóa 1 sản phẩm
        public async Task<APIRespone<bool>> RemoveItemsAsync(List<int> cartIds)
        {
            var items = _context.Carts.Where(c => cartIds.Contains(c.CartId));
            if (!items.Any())
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm",
                    Data = false
                };
            }

            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = "Đã xóa các sản phẩm được chọn",
                Data = true
            };
        }


        // Xóa tất cả
        public async Task<APIRespone<bool>> ClearCartAsync(int userId)
        {
            var items = _context.Carts.Where(c => c.UserId == userId);
            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();

            return new APIRespone<bool> { Success = true, Message = "Đã xóa toàn bộ giỏ hàng", Data = true };
        }

        private async Task<APIRespone<CartDto>> GetCartItemDto(int userId, int productId)
        {
            var item = await _context.Carts.Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item == null) return new APIRespone<CartDto>
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };

            return new APIRespone<CartDto>
            {
                Success = true,
                Message = "Thành công",
                Data = new CartDto
                {
                    CartId = item.CartId,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ImageUrl = item.Product.ImageUrl,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                }
            };
        }
    }
}
