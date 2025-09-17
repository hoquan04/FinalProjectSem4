using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<IEnumerable<Review>>> GetAllAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .ToListAsync();

                return new APIRespone<IEnumerable<Review>>
                {
                    Success = true,
                    Message = "Lấy danh sách đánh giá thành công",
                    Data = reviews
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Review>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách đánh giá: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<Review>> GetByIdAsync(int id)
        {
            try
            {
                var review = await _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.ReviewId == id);

                if (review == null)
                {
                    return new APIRespone<Review>
                    {
                        Success = false,
                        Message = "Không tìm thấy đánh giá",
                        Data = null
                    };
                }

                return new APIRespone<Review>
                {
                    Success = true,
                    Message = "Lấy thông tin đánh giá thành công",
                    Data = review
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Review>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy thông tin đánh giá: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<Review>> AddAsync(Review entity)
        {
            try
            {
                // Kiểm tra sản phẩm có tồn tại không
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == entity.ProductId);
                if (!productExists)
                {
                    return new APIRespone<Review>
                    {
                        Success = false,
                        Message = "Sản phẩm không tồn tại",
                        Data = null
                    };
                }

                // Kiểm tra người dùng có tồn tại không
                var userExists = await _context.Users.AnyAsync(u => u.UserId == entity.UserId);
                if (!userExists)
                {
                    return new APIRespone<Review>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại",
                        Data = null
                    };
                }

                // Kiểm tra người dùng đã đánh giá sản phẩm này chưa
                var existingReview = await _context.Reviews
                    .AnyAsync(r => r.ProductId == entity.ProductId && r.UserId == entity.UserId);

                if (existingReview)
                {
                    return new APIRespone<Review>
                    {
                        Success = false,
                        Message = "Bạn đã đánh giá sản phẩm này rồi",
                        Data = null
                    };
                }

                entity.CreatedAt = DateTime.Now;
                _context.Reviews.Add(entity);
                await _context.SaveChangesAsync();

                var createdReview = await _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.ReviewId == entity.ReviewId);

                return new APIRespone<Review>
                {
                    Success = true,
                    Message = "Thêm đánh giá thành công",
                    Data = createdReview
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Review>
                {
                    Success = false,
                    Message = $"Lỗi khi thêm đánh giá: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<Review>> UpdateAsync(int id, Review entity)
        {
            try
            {
                var existingReview = await _context.Reviews.FindAsync(id);
                if (existingReview == null)
                {
                    return new APIRespone<Review>
                    {
                        Success = false,
                        Message = "Không tìm thấy đánh giá",
                        Data = null
                    };
                }

                // Chỉ cho phép cập nhật rating và comment
                existingReview.Rating = entity.Rating;
                existingReview.Comment = entity.Comment;

                await _context.SaveChangesAsync();

                var updatedReview = await _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.ReviewId == id);

                return new APIRespone<Review>
                {
                    Success = true,
                    Message = "Cập nhật đánh giá thành công",
                    Data = updatedReview
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Review>
                {
                    Success = false,
                    Message = $"Lỗi khi cập nhật đánh giá: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Không tìm thấy đánh giá",
                        Data = false
                    };
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Xóa đánh giá thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi xóa đánh giá: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<PagedResponse<Review>>> GetPageAsync(int pageNow, int pageSize)
        {
            try
            {
                var totalCount = await _context.Reviews.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<Review>
                {
                    Data = reviews,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPages,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<Review>>
                {
                    Success = true,
                    Message = "Lấy danh sách đánh giá phân trang thành công",
                    Data = pagedResponse
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<PagedResponse<Review>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách đánh giá phân trang: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<IEnumerable<Review>>> GetByProductIdAsync(int productId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Users)
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return new APIRespone<IEnumerable<Review>>
                {
                    Success = true,
                    Message = "Lấy danh sách đánh giá theo sản phẩm thành công",
                    Data = reviews
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Review>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách đánh giá theo sản phẩm: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<IEnumerable<Review>>> GetByUserIdAsync(int userId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return new APIRespone<IEnumerable<Review>>
                {
                    Success = true,
                    Message = "Lấy danh sách đánh giá theo người dùng thành công",
                    Data = reviews
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Review>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách đánh giá theo người dùng: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<double>> GetAverageRatingByProductIdAsync(int productId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.ProductId == productId)
                    .ToListAsync();

                if (!reviews.Any())
                {
                    return new APIRespone<double>
                    {
                        Success = true,
                        Message = "Chưa có đánh giá nào cho sản phẩm này",
                        Data = 0
                    };
                }

                var averageRating = reviews.Average(r => r.Rating);

                return new APIRespone<double>
                {
                    Success = true,
                    Message = "Lấy điểm đánh giá trung bình thành công",
                    Data = Math.Round(averageRating, 1)
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<double>
                {
                    Success = false,
                    Message = $"Lỗi khi tính điểm đánh giá trung bình: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<APIRespone<PagedResponse<Review>>> SearchAsync(string? searchKeyword, int pageNow, int pageSize)
        {
            try
            {
                var query = _context.Reviews
                    .Include(r => r.Product)
                    .Include(r => r.Users)
                    .AsQueryable();

                // Tìm kiếm theo keyword trong ProductName hoặc UserFullName hoặc Comment
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    searchKeyword = searchKeyword.ToLower().Trim();
                    query = query.Where(r =>
                        r.Product.Name.ToLower().Contains(searchKeyword) ||
                        r.Users.FullName.ToLower().Contains(searchKeyword) ||
                        (r.Comment != null && r.Comment.ToLower().Contains(searchKeyword))
                    );
                }

                // Đếm tổng số bản ghi sau khi filter
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Lấy dữ liệu phân trang
                var reviews = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pagedResponse = new PagedResponse<Review>
                {
                    Data = reviews,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPages,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<Review>>
                {
                    Success = true,
                    Message = string.IsNullOrEmpty(searchKeyword)
                        ? "Lấy danh sách đánh giá thành công"
                        : $"Tìm kiếm đánh giá với từ khóa '{searchKeyword}' thành công",
                    Data = pagedResponse
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<PagedResponse<Review>>
                {
                    Success = false,
                    Message = $"Lỗi khi tìm kiếm đánh giá: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
