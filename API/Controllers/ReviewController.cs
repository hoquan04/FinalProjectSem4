using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static API.Models.DTO.ReviewDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: api/Review
        [HttpGet]
        public async Task<ActionResult<APIRespone<IEnumerable<ReviewResponseDto>>>> GetAllReviews()
        {
            var result = await _reviewRepository.GetAllAsync();

            if (result.Success && result.Data != null)
            {
                var reviewDtos = result.Data.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name,
                    UserFullName = r.Users?.FullName
                });

                return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = reviewDtos
                });
            }

            return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public async Task<ActionResult<APIRespone<ReviewResponseDto>>> GetReview(int id)
        {
            var result = await _reviewRepository.GetByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                var reviewDto = new ReviewResponseDto
                {
                    ReviewId = result.Data.ReviewId,
                    ProductId = result.Data.ProductId,
                    UserId = result.Data.UserId,
                    Rating = result.Data.Rating,
                    Comment = result.Data.Comment,
                    CreatedAt = result.Data.CreatedAt,
                    ProductName = result.Data.Product?.Name,
                    UserFullName = result.Data.Users?.FullName
                };

                return Ok(new APIRespone<ReviewResponseDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = reviewDto
                });
            }

            return Ok(new APIRespone<ReviewResponseDto>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // GET: api/Review/product/5
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<APIRespone<IEnumerable<ReviewResponseDto>>>> GetReviewsByProduct(int productId)
        {
            var result = await _reviewRepository.GetByProductIdAsync(productId);

            if (result.Success && result.Data != null)
            {
                var reviewDtos = result.Data.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name,
                    UserFullName = r.Users?.FullName
                });

                return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = reviewDtos
                });
            }

            return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // GET: api/Review/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<APIRespone<IEnumerable<ReviewResponseDto>>>> GetReviewsByUser(int userId)
        {
            var result = await _reviewRepository.GetByUserIdAsync(userId);

            if (result.Success && result.Data != null)
            {
                var reviewDtos = result.Data.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name,
                    UserFullName = r.Users?.FullName
                });

                return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = reviewDtos
                });
            }

            return Ok(new APIRespone<IEnumerable<ReviewResponseDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // GET: api/Review/average/5
        [HttpGet("average/{productId}")]
        public async Task<ActionResult<APIRespone<double>>> GetAverageRating(int productId)
        {
            var result = await _reviewRepository.GetAverageRatingByProductIdAsync(productId);
            return Ok(result);
        }

        // GET: api/Review/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<ActionResult<APIRespone<PagedResponse<ReviewResponseDto>>>> GetReviewsPage([FromQuery] int pageNow = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNow <= 0 || pageSize <= 0)
            {
                return BadRequest(new APIRespone<PagedResponse<ReviewResponseDto>>
                {
                    Success = false,
                    Message = "PageNow và PageSize phải lớn hơn 0",
                    Data = null
                });
            }

            var result = await _reviewRepository.GetPageAsync(pageNow, pageSize);

            if (result.Success && result.Data != null)
            {
                var reviewDtos = result.Data.Data.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name,
                    UserFullName = r.Users?.FullName
                });

                var pagedResponseDto = new PagedResponse<ReviewResponseDto>
                {
                    Data = reviewDtos,
                    PageNow = result.Data.PageNow,
                    PageSize = result.Data.PageSize,
                    TotalPage = result.Data.TotalPage,
                    TotalCount = result.Data.TotalCount
                };

                return Ok(new APIRespone<PagedResponse<ReviewResponseDto>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = pagedResponseDto
                });
            }

            return Ok(new APIRespone<PagedResponse<ReviewResponseDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // POST: api/Review
        [HttpPost]
        public async Task<ActionResult<APIRespone<ReviewResponseDto>>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = new Review
            {
                ProductId = createReviewDto.ProductId,
                UserId = createReviewDto.UserId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment
            };

            var result = await _reviewRepository.AddAsync(review);

            if (result.Success && result.Data != null)
            {
                var reviewDto = new ReviewResponseDto
                {
                    ReviewId = result.Data.ReviewId,
                    ProductId = result.Data.ProductId,
                    UserId = result.Data.UserId,
                    Rating = result.Data.Rating,
                    Comment = result.Data.Comment,
                    CreatedAt = result.Data.CreatedAt,
                    ProductName = result.Data.Product?.Name,
                    UserFullName = result.Data.Users?.FullName
                };

                return CreatedAtAction(nameof(GetReview), new { id = reviewDto.ReviewId },
                    new APIRespone<ReviewResponseDto>
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Data = reviewDto
                    });
            }

            return BadRequest(new APIRespone<ReviewResponseDto>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // PUT: api/Review/5
        [HttpPut("{id}")]
        public async Task<ActionResult<APIRespone<ReviewResponseDto>>> UpdateReview(int id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = new Review
            {
                Rating = updateReviewDto.Rating,
                Comment = updateReviewDto.Comment
            };

            var result = await _reviewRepository.UpdateAsync(id, review);

            if (result.Success && result.Data != null)
            {
                var reviewDto = new ReviewResponseDto
                {
                    ReviewId = result.Data.ReviewId,
                    ProductId = result.Data.ProductId,
                    UserId = result.Data.UserId,
                    Rating = result.Data.Rating,
                    Comment = result.Data.Comment,
                    CreatedAt = result.Data.CreatedAt,
                    ProductName = result.Data.Product?.Name,
                    UserFullName = result.Data.Users?.FullName
                };

                return Ok(new APIRespone<ReviewResponseDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = reviewDto
                });
            }

            return Ok(new APIRespone<ReviewResponseDto>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }

        // DELETE: api/Review/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIRespone<bool>>> DeleteReview(int id)
        {
            var result = await _reviewRepository.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<APIRespone<PagedResponse<ReviewResponseDto>>>> SearchReviews(
    [FromQuery] string? searchKeyword,
    [FromQuery] int pageNow = 1,
    [FromQuery] int pageSize = 10)
        {
            if (pageNow <= 0 || pageSize <= 0)
            {
                return BadRequest(new APIRespone<PagedResponse<ReviewResponseDto>>
                {
                    Success = false,
                    Message = "PageNow và PageSize phải lớn hơn 0",
                    Data = null
                });
            }

            var result = await _reviewRepository.SearchAsync(searchKeyword, pageNow, pageSize);

            if (result.Success && result.Data != null)
            {
                var reviewDtos = result.Data.Data.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name,
                    UserFullName = r.Users?.FullName
                });

                var pagedResponseDto = new PagedResponse<ReviewResponseDto>
                {
                    Data = reviewDtos,
                    PageNow = result.Data.PageNow,
                    PageSize = result.Data.PageSize,
                    TotalPage = result.Data.TotalPage,
                    TotalCount = result.Data.TotalCount
                };

                return Ok(new APIRespone<PagedResponse<ReviewResponseDto>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = pagedResponseDto
                });
            }

            return Ok(new APIRespone<PagedResponse<ReviewResponseDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null
            });
        }
    }
}
