using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly DataContext _context;

        public FavoriteRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<bool>> AddToFavoritesAsync(int userId, int productId)
        {
            try
            {
                // Kiểm tra user có tồn tại không
                var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại",
                        Data = false
                    };
                }

                // Kiểm tra product có tồn tại không
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
                if (!productExists)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Sản phẩm không tồn tại",
                        Data = false
                    };
                }

                // Kiểm tra đã yêu thích chưa
                var existingFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

                if (existingFavorite)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Sản phẩm đã có trong danh sách yêu thích",
                        Data = false
                    };
                }

                // Thêm vào favorites
                var favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.Now
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Đã thêm sản phẩm vào danh sách yêu thích",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi thêm sản phẩm yêu thích: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<bool>> RemoveFromFavoritesAsync(int userId, int productId)
        {
            try
            {
                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

                if (favorite == null)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Sản phẩm không có trong danh sách yêu thích",
                        Data = false
                    };
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Đã xóa sản phẩm khỏi danh sách yêu thích",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi xóa sản phẩm yêu thích: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<bool>> CheckIsFavoriteAsync(int userId, int productId)
        {
            try
            {
                var isFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Kiểm tra trạng thái yêu thích thành công",
                    Data = isFavorite
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi kiểm tra trạng thái yêu thích: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<IEnumerable<Favorite>>> GetUserFavoritesAsync(int userId)
        {
            try
            {
                var favorites = await _context.Favorites
                    .Include(f => f.Product)
                        .ThenInclude(p => p.Category)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                return new APIRespone<IEnumerable<Favorite>>
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm yêu thích thành công",
                    Data = favorites
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Favorite>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách sản phẩm yêu thích: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<PagedResponse<Favorite>>> GetUserFavoritesPagedAsync(int userId, int pageNow, int pageSize)
        {
            try
            {
                var totalCount = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var favorites = await _context.Favorites
                    .Include(f => f.Product)
                        .ThenInclude(p => p.Category)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<Favorite>
                {
                    Data = favorites,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPages,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<Favorite>>
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm yêu thích phân trang thành công",
                    Data = pagedResponse
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<PagedResponse<Favorite>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách sản phẩm yêu thích phân trang: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<int>> GetFavoriteCountByProductIdAsync(int productId)
        {
            try
            {
                var count = await _context.Favorites
                    .Where(f => f.ProductId == productId)
                    .CountAsync();

                return new APIRespone<int>
                {
                    Success = true,
                    Message = "Lấy số lượng yêu thích thành công",
                    Data = count
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<int>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy số lượng yêu thích: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<APIRespone<bool>> ClearAllFavoritesAsync(int userId)
        {
            try
            {
                var favorites = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                if (!favorites.Any())
                {
                    return new APIRespone<bool>
                    {
                        Success = true,
                        Message = "Danh sách yêu thích đã trống",
                        Data = true
                    };
                }

                _context.Favorites.RemoveRange(favorites);
                await _context.SaveChangesAsync();

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Đã xóa tất cả sản phẩm yêu thích",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi xóa tất cả sản phẩm yêu thích: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<IEnumerable<Favorite>>> GetMostFavoriteProductsAsync(int count = 10)
        {
            try
            {
                var mostFavoriteProducts = await _context.Favorites
                    .Include(f => f.Product)
                        .ThenInclude(p => p.Category)
                    .GroupBy(f => f.ProductId)
                    .OrderByDescending(g => g.Count())
                    .Take(count)
                    .SelectMany(g => g.Take(1)) // Lấy 1 favorite từ mỗi group
                    .ToListAsync();

                return new APIRespone<IEnumerable<Favorite>>
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm được yêu thích nhiều nhất thành công",
                    Data = mostFavoriteProducts
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Favorite>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách sản phẩm được yêu thích nhiều nhất: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
