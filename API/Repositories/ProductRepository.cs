using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        // Thêm mới sản phẩm
        public async Task<APIRespone<Product>> AddAsync(Product entity)
        {
            var response = new APIRespone<Product>();

            try
            {
                // Kiểm tra CategoryId có tồn tại không
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == entity.CategoryId);
                if (!categoryExists)
                {
                    response.Success = false;
                    response.Message = "Danh mục không tồn tại";
                    return response;
                }

                _context.Products.Add(entity);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Thêm sản phẩm thành công";
                response.Data = entity;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi thêm sản phẩm: {ex.Message}";
            }

            return response;
        }

        // Xóa sản phẩm
        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var response = new APIRespone<bool>();

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm";
                    response.Data = false;
                    return response;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Xóa sản phẩm thành công";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi xóa sản phẩm: {ex.Message}";
                response.Data = false;
            }

            return response;
        }

        // Lấy tất cả sản phẩm
        public async Task<APIRespone<IEnumerable<Product>>> GetAllAsync()
        {
            var response = new APIRespone<IEnumerable<Product>>();

            try
            {
                var data = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                response.Success = true;
                response.Data = data;
                response.Message = "Lấy danh sách sản phẩm thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy danh sách sản phẩm: {ex.Message}";
            }

            return response;
        }

        // Lấy sản phẩm theo ID
        public async Task<APIRespone<Product>> GetByIdAsync(int id)
        {
            var response = new APIRespone<Product>();

            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm";
                }
                else
                {
                    response.Success = true;
                    response.Data = product;
                    response.Message = "Lấy thông tin sản phẩm thành công";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy sản phẩm: {ex.Message}";
            }

            return response;
        }

        // Phân trang sản phẩm
        public async Task<APIRespone<PagedResponse<Product>>> GetPageAsync(int pageNow, int pageSize)
        {
            var response = new APIRespone<PagedResponse<Product>>();

            try
            {
                var totalCount = await _context.Products.CountAsync();
                var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

                var data = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pageResponse = new PagedResponse<Product>
                {
                    Data = data,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                response.Success = true;
                response.Data = pageResponse;
                response.Message = "Lấy dữ liệu phân trang sản phẩm thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi phân trang sản phẩm: {ex.Message}";
            }

            return response;
        }

        // Cập nhật sản phẩm
        public async Task<APIRespone<Product>> UpdateAsync(int id, Product entity)
        {
            var response = new APIRespone<Product>();

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm";
                    return response;
                }

                // Kiểm tra CategoryId có tồn tại không
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == entity.CategoryId);
                if (!categoryExists)
                {
                    response.Success = false;
                    response.Message = "Danh mục không tồn tại";
                    return response;
                }

                product.Name = entity.Name;
                product.Description = entity.Description;
                product.Price = entity.Price;
                product.StockQuantity = entity.StockQuantity;
                product.ImageUrl = entity.ImageUrl;
                product.CategoryId = entity.CategoryId;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                // Load category để trả về
                await _context.Entry(product)
                    .Reference(p => p.Category)
                    .LoadAsync();

                response.Success = true;
                response.Message = "Cập nhật sản phẩm thành công";
                response.Data = product;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}";
            }

            return response;
        }

        // Lấy sản phẩm theo danh mục
        public async Task<APIRespone<IEnumerable<Product>>> GetProductsByCategoryAsync(int categoryId)
        {
            var response = new APIRespone<IEnumerable<Product>>();

            try
            {
                var data = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                response.Success = true;
                response.Data = data;
                response.Message = "Lấy sản phẩm theo danh mục thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy sản phẩm theo danh mục: {ex.Message}";
            }

            return response;
        }

        // Tìm kiếm sản phẩm
        public async Task<APIRespone<IEnumerable<Product>>> SearchProductsAsync(string searchTerm)
        {
            var response = new APIRespone<IEnumerable<Product>>();

            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => 
                        p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)) ||
                        p.Category.Name.Contains(searchTerm));
                }

                var data = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                response.Success = true;
                response.Data = data;
                response.Message = "Tìm kiếm sản phẩm thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi tìm kiếm sản phẩm: {ex.Message}";
            }

            return response;
        }
    }
}
