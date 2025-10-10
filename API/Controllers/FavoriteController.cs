using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        // POST: api/favorite
        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDto.AddFavoriteDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _favoriteRepository.AddToFavoritesAsync(model.UserId, model.ProductId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // DELETE: api/favorite
        [HttpDelete]
        public async Task<IActionResult> RemoveFromFavorites([FromQuery] int userId, [FromQuery] int productId)
        {
            if (userId <= 0 || productId <= 0)
                return BadRequest(new { Success = false, Message = "UserId và ProductId phải lớn hơn 0" });

            var response = await _favoriteRepository.RemoveFromFavoritesAsync(userId, productId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // GET: api/favorite/check?userId=1&productId=2
        [HttpGet("check")]
        public async Task<IActionResult> CheckIsFavorite([FromQuery] int userId, [FromQuery] int productId)
        {
            if (userId <= 0 || productId <= 0)
                return BadRequest(new { Success = false, Message = "UserId và ProductId phải lớn hơn 0" });

            var response = await _favoriteRepository.CheckIsFavoriteAsync(userId, productId);
            return Ok(response);
        }

        // GET: api/favorite/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFavorites(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { Success = false, Message = "UserId phải lớn hơn 0" });

            var response = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return Ok(response);
        }

        // GET: api/favorite/user/5/page?pageNow=1&pageSize=10
        [HttpGet("user/{userId}/page")]
        public async Task<IActionResult> GetUserFavoritesPaged(int userId, [FromQuery] int pageNow = 1, [FromQuery] int pageSize = 10)
        {
            if (userId <= 0)
                return BadRequest(new { Success = false, Message = "UserId phải lớn hơn 0" });

            if (pageNow <= 0) pageNow = 1;
            if (pageSize <= 0) pageSize = 10;

            var response = await _favoriteRepository.GetUserFavoritesPagedAsync(userId, pageNow, pageSize);
            return Ok(response);
        }

        // GET: api/favorite/product/5/count
        [HttpGet("product/{productId}/count")]
        public async Task<IActionResult> GetFavoriteCountByProduct(int productId)
        {
            if (productId <= 0)
                return BadRequest(new { Success = false, Message = "ProductId phải lớn hơn 0" });

            var response = await _favoriteRepository.GetFavoriteCountByProductIdAsync(productId);
            return Ok(response);
        }

        // DELETE: api/favorite/user/5/clear
        [HttpDelete("user/{userId}/clear")]
        public async Task<IActionResult> ClearAllFavorites(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { Success = false, Message = "UserId phải lớn hơn 0" });

            var response = await _favoriteRepository.ClearAllFavoritesAsync(userId);
            return Ok(response);
        }

        // GET: api/favorite/most-popular?count=10
        [HttpGet("most-popular")]
        public async Task<IActionResult> GetMostFavoriteProducts([FromQuery] int count = 10)
        {
            if (count <= 0) count = 10;
            if (count > 100) count = 100; // Giới hạn tối đa

            var response = await _favoriteRepository.GetMostFavoriteProductsAsync(count);
            return Ok(response);
        }

        // POST: api/favorite/toggle
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteDto.AddFavoriteDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trạng thái hiện tại
            var checkResponse = await _favoriteRepository.CheckIsFavoriteAsync(model.UserId, model.ProductId);
            if (!checkResponse.Success)
                return BadRequest(checkResponse);

            if (checkResponse.Data) // Đã yêu thích -> xóa
            {
                var removeResponse = await _favoriteRepository.RemoveFromFavoritesAsync(model.UserId, model.ProductId);
                return Ok(new 
                { 
                    Success = removeResponse.Success, 
                    Message = removeResponse.Message, 
                    Data = new { IsFavorite = false, Action = "removed" }
                });
            }
            else // Chưa yêu thích -> thêm
            {
                var addResponse = await _favoriteRepository.AddToFavoritesAsync(model.UserId, model.ProductId);
                return Ok(new 
                { 
                    Success = addResponse.Success, 
                    Message = addResponse.Message, 
                    Data = new { IsFavorite = true, Action = "added" }
                });
            }
        }
    }
}
