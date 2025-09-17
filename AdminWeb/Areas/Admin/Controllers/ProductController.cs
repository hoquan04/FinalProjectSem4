using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET: /Admin/Product - Hiển thị danh sách products
        /// </summary>
        public async Task<IActionResult> Index(string? searchString, int? categoryId)
        {
            Console.WriteLine($"📋 [ProductController.Index] Bắt đầu - SearchString: {searchString}, CategoryId: {categoryId}");
            
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;

            try
            {
                List<ProductViewModel> products;

                // Gọi API thông qua ProductService
                if (!string.IsNullOrEmpty(searchString) || categoryId.HasValue)
                {
                    Console.WriteLine("🔍 [ProductController.Index] Thực hiện tìm kiếm sản phẩm");
                    var searchModel = new ProductSearchModel 
                    { 
                        SearchTerm = searchString,
                        CategoryId = categoryId
                    };
                    products = await _productService.SearchProductsAsync(searchModel);
                    Console.WriteLine($"✅ [ProductController.Index] Tìm kiếm hoàn tất. Tìm thấy {products.Count} sản phẩm");
                }
                else
                {
                    Console.WriteLine("📦 [ProductController.Index] Lấy tất cả sản phẩm");
                    products = await _productService.GetAllProductsAsync();
                    Console.WriteLine($"✅ [ProductController.Index] Lấy tất cả sản phẩm hoàn tất. Tổng: {products.Count} sản phẩm");
                }

                // Lấy categories để hiển thị trong dropdown
                Console.WriteLine("📂 [ProductController.Index] Đang lấy danh sách categories");
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                Console.WriteLine($"✅ [ProductController.Index] Lấy categories hoàn tất. Tổng: {categories.Count} categories");

                return View(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Index] Lỗi: {ex.Message}");
                ViewBag.Error = $"Lỗi khi tải danh sách sản phẩm: {ex.Message}";
                ViewBag.ErrorDetail = "Vui lòng kiểm tra API đã chạy chưa hoặc kết nối mạng.";
                ViewBag.Categories = new List<CategoryViewModel>();
                return View(new List<ProductViewModel>());
            }
        }

        /// <summary>
        /// GET: /Admin/Product/Create - Hiển thị form tạo mới
        /// </summary>
        public async Task<IActionResult> Create()
        {
            Console.WriteLine("➕ [ProductController.Create GET] Hiển thị form tạo sản phẩm mới");

            try
            {
                // Lấy categories để hiển thị trong dropdown
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                
                Console.WriteLine($"✅ [ProductController.Create GET] Lấy {categories.Count} categories thành công");
                return View(new ProductCreateModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Create GET] Lỗi: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Admin/Product/Create - Xử lý tạo product mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel model)
        {
            Console.WriteLine($"💾 [ProductController.Create POST] Bắt đầu tạo sản phẩm: {model.Name}");
            Console.WriteLine($"📊 [ProductController.Create POST] CategoryId: {model.CategoryId}, Price: {model.Price}, Stock: {model.StockQuantity}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ [ProductController.Create POST] Model validation failed");
                
                // Reload categories nếu có lỗi validation
                try
                {
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                }
                catch
                {
                    ViewBag.Categories = new List<CategoryViewModel>();
                }
                return View(model);
            }

            try
            {
                Console.WriteLine("🚀 [ProductController.Create POST] Gọi ProductService.CreateProductAsync");
                var result = await _productService.CreateProductAsync(model);
                
                Console.WriteLine($"📊 [ProductController.Create POST] API Response - Success: {result.Success}, Message: {result.Message}");
                
                if (result.Success)
                {
                    Console.WriteLine("✅ [ProductController.Create POST] Tạo sản phẩm thành công!");
                    TempData["SuccessMessage"] = result.Message ?? "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Console.WriteLine($"⚠️ [ProductController.Create POST] Tạo sản phẩm thất bại: {result.Message}");
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi thêm sản phẩm");
                    
                    // Reload categories
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Create POST] Exception: {ex.Message}");
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                
                // Reload categories
                try
                {
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                }
                catch
                {
                    ViewBag.Categories = new List<CategoryViewModel>();
                }
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Admin/Product/Edit/5 - Hiển thị form chỉnh sửa
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            Console.WriteLine($"✏️ [ProductController.Edit GET] Hiển thị form chỉnh sửa sản phẩm ID: {id}");

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    Console.WriteLine($"⚠️ [ProductController.Edit GET] Không tìm thấy sản phẩm ID: {id}");
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"✅ [ProductController.Edit GET] Lấy sản phẩm thành công: {product.Name}");

                // Convert sang EditModel
                var editModel = new ProductEditModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId
                };

                // Lấy categories để hiển thị trong dropdown
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                
                Console.WriteLine($"✅ [ProductController.Edit GET] Lấy {categories.Count} categories thành công");
                return View(editModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Edit GET] Lỗi: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi tải sản phẩm: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Admin/Product/Edit/5 - Xử lý cập nhật product
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditModel model)
        {
            Console.WriteLine($"💾 [ProductController.Edit POST] Bắt đầu cập nhật sản phẩm ID: {id}");
            Console.WriteLine($"📊 [ProductController.Edit POST] Name: {model.Name}, CategoryId: {model.CategoryId}");

            if (id != model.ProductId)
            {
                Console.WriteLine($"⚠️ [ProductController.Edit POST] ID mismatch - URL: {id}, Model: {model.ProductId}");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ [ProductController.Edit POST] Model validation failed");
                try
                {
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                }
                catch
                {
                    ViewBag.Categories = new List<CategoryViewModel>();
                }
                return View(model);
            }

            try
            {
                Console.WriteLine("🚀 [ProductController.Edit POST] Gọi ProductService.UpdateProductAsync");
                var result = await _productService.UpdateProductAsync(id, model);
                
                Console.WriteLine($"📊 [ProductController.Edit POST] API Response - Success: {result.Success}");
                
                if (result.Success)
                {
                    Console.WriteLine("✅ [ProductController.Edit POST] Cập nhật sản phẩm thành công!");
                    TempData["SuccessMessage"] = result.Message ?? "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Console.WriteLine($"⚠️ [ProductController.Edit POST] Cập nhật thất bại: {result.Message}");
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi cập nhật sản phẩm");
                    
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Edit POST] Exception: {ex.Message}");
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                
                try
                {
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Categories = categories;
                }
                catch
                {
                    ViewBag.Categories = new List<CategoryViewModel>();
                }
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Admin/Product/Delete/5 - Hiển thị xác nhận xóa
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine($"🗑️ [ProductController.Delete GET] Hiển thị form xóa sản phẩm ID: {id}");

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    Console.WriteLine($"⚠️ [ProductController.Delete GET] Không tìm thấy sản phẩm ID: {id}");
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction(nameof(Index));
                }
                
                Console.WriteLine($"✅ [ProductController.Delete GET] Lấy sản phẩm thành công: {product.Name}");
                return View(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.Delete GET] Lỗi: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi tải sản phẩm: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Admin/Product/Delete/5 - Xử lý xóa product
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"🗑️ [ProductController.DeleteConfirmed] Bắt đầu xóa sản phẩm ID: {id}");

            try
            {
                Console.WriteLine("🚀 [ProductController.DeleteConfirmed] Gọi ProductService.DeleteProductAsync");
                var result = await _productService.DeleteProductAsync(id);
                
                Console.WriteLine($"📊 [ProductController.DeleteConfirmed] API Response - Success: {result.Success}");
                
                if (result.Success)
                {
                    Console.WriteLine("✅ [ProductController.DeleteConfirmed] Xóa sản phẩm thành công!");
                    TempData["SuccessMessage"] = result.Message ?? "Xóa sản phẩm thành công!";
                }
                else
                {
                    Console.WriteLine($"⚠️ [ProductController.DeleteConfirmed] Xóa thất bại: {result.Message}");
                    TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi xóa sản phẩm";
                }
                
                Console.WriteLine("🎯 [ProductController.DeleteConfirmed] Redirect to Index");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductController.DeleteConfirmed] Exception: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
