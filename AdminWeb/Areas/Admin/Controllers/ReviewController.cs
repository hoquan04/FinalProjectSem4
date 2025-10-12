using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {

        private readonly IReviewApiService _reviewApiService;

        public ReviewController(IReviewApiService reviewApiService)
        {
            _reviewApiService = reviewApiService;
        }

        // GET: Review
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _reviewApiService.GetPagedAsync(page, pageSize);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                // SỬA: Return empty List<ReviewViewModel> thay vì ReviewSearchViewModel
                return View(new List<ReviewViewModel>());
            }

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = response.Data?.TotalPage ?? 0;
            ViewBag.TotalCount = response.Data?.TotalCount ?? 0;
            // THÊM: Tạo empty search model để View có thể access
            ViewBag.SearchModel = new ReviewSearchViewModel();

            return View(response.Data?.Data ?? new List<ReviewViewModel>());
        }

        // GET: Review/Search
        public async Task<IActionResult> Search(ReviewSearchViewModel searchModel)
        {
            if (searchModel.PageNumber < 1) searchModel.PageNumber = 1;
            if (searchModel.PageSize < 1) searchModel.PageSize = 10;

            var response = await _reviewApiService.SearchAsync(searchModel);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View("Index", new List<ReviewViewModel>());
            }

            ViewBag.CurrentPage = searchModel.PageNumber;
            ViewBag.PageSize = searchModel.PageSize;
            ViewBag.TotalPages = response.Data?.TotalPage ?? 0;
            ViewBag.TotalCount = response.Data?.TotalCount ?? 0;
            ViewBag.SearchModel = searchModel;

            return View("Index", response.Data?.Data ?? new List<ReviewViewModel>());
        }

        // GET: Review/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _reviewApiService.GetByIdAsync(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["ErrorMessage"] = response.Message ?? "Không tìm thấy đánh giá";
                    return RedirectToAction(nameof(Index));
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Review/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _reviewApiService.CreateAsync(model);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Tạo đánh giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message;
            return View(model);
        }

        // GET: Review/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _reviewApiService.GetByIdAsync(id);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Không tìm thấy đánh giá";
                return RedirectToAction(nameof(Index));
            }

            var updateModel = new UpdateReviewViewModel
            {
                ReviewId = response.Data.ReviewId,
                Rating = response.Data.Rating,
                Comment = response.Data.Comment
            };

            return View(updateModel);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateReviewViewModel model)
        {
            if (id != model.ReviewId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _reviewApiService.UpdateAsync(id, model);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Cập nhật đánh giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = response.Message;
            return View(model);
        }

        // GET: Review/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _reviewApiService.GetByIdAsync(id);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Không tìm thấy đánh giá";
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        // POST: Review/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _reviewApiService.DeleteAsync(id);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Xóa đánh giá thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Review/ByProduct/5
        public async Task<IActionResult> ByProduct(int productId)
        {
            var response = await _reviewApiService.GetByProductIdAsync(productId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View(new List<ReviewViewModel>());
            }

            ViewBag.ProductId = productId;
            return View(response.Data ?? new List<ReviewViewModel>());
        }

        // GET: Review/ByUser/5
        public async Task<IActionResult> ByUser(int userId)
        {
            var response = await _reviewApiService.GetByUserIdAsync(userId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View(new List<ReviewViewModel>());
            }

            ViewBag.UserId = userId;
            return View(response.Data ?? new List<ReviewViewModel>());
        }

        // AJAX: Get average rating for product
        [HttpGet]
        public async Task<IActionResult> GetAverageRating(int productId)
        {
            var response = await _reviewApiService.GetAverageRatingByProductIdAsync(productId);
            return Json(response);
        }
    }
}
