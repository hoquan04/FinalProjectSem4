//using API.Data;
//using API.Models;
//using API.Repositories.IRepositories;
//using API.Repositories.RestAPI;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace API.Repositories
//{
//    public class CategoryRepository : ICategoryRepository
//    {
//        private readonly DataContext _context;

//        public CategoryRepository(DataContext context)
//        {
//            _context = context;
//        }

//        // Thêm mới
//        public async Task<APIRespone<Category>> AddAsync(Category entity)
//        {
//            var response = new APIRespone<Category>();

//            try
//            {
//                _context.Categories.Add(entity);
//                await _context.SaveChangesAsync();

//                response.Success = true;
//                response.Message = "Thêm danh mục thành công";
//                response.Data = entity;
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi thêm danh mục: {ex.Message}";
//            }

//            return response;
//        }

//        // Xóa
//        public async Task<APIRespone<bool>> DeleteAsync(int id)
//        {
//            var response = new APIRespone<bool>();

//            try
//            {
//                var category = await _context.Categories.FindAsync(id);
//                if (category == null)
//                {
//                    response.Success = false;
//                    response.Message = "Không tìm thấy danh mục";
//                    response.Data = false;
//                    return response;
//                }

//                _context.Categories.Remove(category);
//                await _context.SaveChangesAsync();

//                response.Success = true;
//                response.Message = "Xóa danh mục thành công";
//                response.Data = true;
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi xóa danh mục: {ex.Message}";
//                response.Data = false;
//            }

//            return response;
//        }

//        // Lấy tất cả
//        public async Task<APIRespone<IEnumerable<Category>>> GetAllAsync()
//        {
//            var response = new APIRespone<IEnumerable<Category>>();

//            try
//            {
//                var data = await _context.Categories.ToListAsync();
//                response.Success = true;
//                response.Data = data;
//                response.Message = "Lấy danh mục thành công";
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi lấy danh mục: {ex.Message}";
//            }

//            return response;
//        }

//        // Lấy theo ID
//        public async Task<APIRespone<Category>> GetByIdAsync(int id)
//        {
//            var response = new APIRespone<Category>();

//            try
//            {
//                var category = await _context.Categories.FindAsync(id);
//                if (category == null)
//                {
//                    response.Success = false;
//                    response.Message = "Không tìm thấy danh mục";
//                }
//                else
//                {
//                    response.Success = true;
//                    response.Data = category;
//                }
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi lấy danh mục: {ex.Message}";
//            }

//            return response;
//        }

//        // Phân trang
//        public async Task<APIRespone<PagedResponse<Category>>> GetPageAsync(int pageNow, int pageSize)
//        {
//            var response = new APIRespone<PagedResponse<Category>>();

//            try
//            {
//                var totalCount = await _context.Categories.CountAsync();
//                var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

//                var data = await _context.Categories
//                    .OrderBy(c => c.CategoryId)
//                    .Skip((pageNow - 1) * pageSize)
//                    .Take(pageSize)
//                    .ToListAsync();

//                var pageResponse = new PagedResponse<Category>
//                {
//                    Data = data,
//                    PageNow = pageNow,
//                    PageSize = pageSize,
//                    TotalPage = totalPage,
//                    TotalCount = totalCount
//                };

//                response.Success = true;
//                response.Data = pageResponse;
//                response.Message = "Lấy dữ liệu phân trang thành công";
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi phân trang: {ex.Message}";
//            }

//            return response;
//        }

//        // Cập nhật
//        public async Task<APIRespone<Category>> UpdateAsync(int id, Category entity)
//        {
//            var response = new APIRespone<Category>();

//            try
//            {
//                var category = await _context.Categories.FindAsync(id);
//                if (category == null)
//                {
//                    response.Success = false;
//                    response.Message = "Không tìm thấy danh mục";
//                    return response;
//                }

//                category.Name = entity.Name;
//                category.Description = entity.Description;

//                _context.Categories.Update(category);
//                await _context.SaveChangesAsync();

//                response.Success = true;
//                response.Message = "Cập nhật danh mục thành công";
//                response.Data = category;
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = $"Lỗi khi cập nhật danh mục: {ex.Message}";
//            }

//            return response;
//        }
//    }
//}
