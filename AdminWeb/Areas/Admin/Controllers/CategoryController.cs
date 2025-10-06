using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET: /Admin/Category - Hiển thị danh sách categories
        /// </summary>
        public async Task<IActionResult> Index(string? searchString, int pageNow = 1, int pageSize = 2)
        {
            ViewBag.SearchString = searchString;
            ViewBag.PageNow = pageNow;
            ViewBag.PageSize = pageSize;

            try
            {
                PagedResponse<CategoryViewModel> pagedCategories;

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Nếu có search, dùng logic cũ
                    var searchModel = new CategorySearchModel { SearchTerm = searchString };
                    var searchResults = await _categoryService.SearchCategoriesAsync(searchModel);
                    pagedCategories = new PagedResponse<CategoryViewModel>
                    {
                        Data = searchResults,
                        PageNow = 1,
                        PageSize = searchResults.Count,
                        TotalCount = searchResults.Count,
                        TotalPage = 1
                    };
                }
                else
                {
                    // Gọi API phân trang
                    pagedCategories = await _categoryService.GetCategoriesPagedAsync(pageNow, pageSize);
                }

                return View(pagedCategories);
            }
            catch (Exception ex)
            {
                // Error handling
                ViewBag.Error = $"Lỗi khi tải danh sách danh mục: {ex.Message}";
                ViewBag.ErrorDetail = "Vui lòng kiểm tra API đã chạy chưa hoặc kết nối mạng.";
                return View(new PagedResponse<CategoryViewModel>());
            }
        }

        /// <summary>
        /// GET: /Admin/Category/Create - Hiển thị form tạo mới
        /// </summary>
        public IActionResult Create()
        {
            return View(new CategoryCreateModel());
        }

        /// <summary>
        /// POST: /Admin/Category/Create - Xử lý tạo category mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Gọi API POST thông qua CategoryService
                var result = await _categoryService.CreateCategoryAsync(model);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Thêm danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi thêm danh mục");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Admin/Category/Edit/5 - Hiển thị form chỉnh sửa
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Gọi API GET by ID thông qua CategoryService
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy danh mục";
                    return RedirectToAction(nameof(Index));
                }

                // Convert sang EditModel
                var editModel = new CategoryEditModel
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh mục: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Admin/Category/Edit/5 - Xử lý cập nhật category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditModel model)
        {
            if (id != model.CategoryId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Gọi API PUT thông qua CategoryService
                var result = await _categoryService.UpdateCategoryAsync(id, model);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Cập nhật danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi cập nhật danh mục");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Admin/Category/Delete/5 - Hiển thị xác nhận xóa
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy danh mục";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh mục: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Admin/Category/Delete/5 - Xử lý xóa category
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Xóa danh mục thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi xóa danh mục";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
