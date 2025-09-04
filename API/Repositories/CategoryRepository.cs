using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        // Thêm mới
        public async Task<APIRespone<CategoryModel>> AddAsync(CategoryModel entity)
        {
            var response = new APIRespone<CategoryModel>();

            try
            {
                _context.Categories.Add(entity);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Thêm danh mục thành công";
                response.Data = entity;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi thêm danh mục: {ex.Message}";
            }

            return response;
        }

        // Xóa
        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var response = new APIRespone<bool>();

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy danh mục";
                    response.Data = false;
                    return response;
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Xóa danh mục thành công";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi xóa danh mục: {ex.Message}";
                response.Data = false;
            }

            return response;
        }

        // Lấy tất cả
        public async Task<APIRespone<IEnumerable<CategoryModel>>> GetAllAsync()
        {
            var response = new APIRespone<IEnumerable<CategoryModel>>();

            try
            {
                var data = await _context.Categories.ToListAsync();
                response.Success = true;
                response.Data = data;
                response.Message = "Lấy danh mục thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy danh mục: {ex.Message}";
            }

            return response;
        }

        // Lấy theo ID
        public async Task<APIRespone<CategoryModel>> GetByIdAsync(int id)
        {
            var response = new APIRespone<CategoryModel>();

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy danh mục";
                }
                else
                {
                    response.Success = true;
                    response.Data = category;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy danh mục: {ex.Message}";
            }

            return response;
        }

        // Phân trang
        public async Task<APIRespone<PagedResponse<CategoryModel>>> GetPageAsync(int pageNow, int pageSize)
        {
            var response = new APIRespone<PagedResponse<CategoryModel>>();

            try
            {
                var totalCount = await _context.Categories.CountAsync();
                var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

                var data = await _context.Categories
                    .OrderBy(c => c.Id)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pageResponse = new PagedResponse<CategoryModel>
                {
                    Data = data,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                response.Success = true;
                response.Data = pageResponse;
                response.Message = "Lấy dữ liệu phân trang thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi phân trang: {ex.Message}";
            }

            return response;
        }

        // Cập nhật
        public async Task<APIRespone<CategoryModel>> UpdateAsync(int id, CategoryModel entity)
        {
            var response = new APIRespone<CategoryModel>();

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy danh mục";
                    return response;
                }

                category.Title = entity.Title;
                category.image = entity.image;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Cập nhật danh mục thành công";
                response.Data = category;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi cập nhật danh mục: {ex.Message}";
            }

            return response;
        }
    }
}
