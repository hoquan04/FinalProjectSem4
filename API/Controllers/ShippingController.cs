using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingService _shippingService;

        public ShippingController(ShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        // GET: api/Shipping
        [HttpGet]
        public async Task<IActionResult> GetAllShippings()
        {
            var result = await _shippingService.GetAllShippingsAsync();
            return Ok(result);
        }

        // GET: api/Shipping/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShippingById(int id)
        {
            var result = await _shippingService.GetShippingByIdAsync(id);
            return Ok(result);
        }

        // POST: api/Shipping
        [HttpPost]
        public async Task<IActionResult> CreateShipping([FromBody] ShippingCreateDto shippingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to Entity
            var shipping = new Shipping
            {
                Address = shippingDto.Address,
                City = shippingDto.City,
                PostalCode = shippingDto.PostalCode,
                ShippingFee = shippingDto.ShippingFee,
                EstimatedDays = shippingDto.EstimatedDays
            };

            var result = await _shippingService.CreateShippingAsync(shipping);
            return Ok(result);
        }

        // PUT: api/Shipping/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipping(int id, [FromBody] ShippingUpdateDto shippingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to Entity
            var shipping = new Shipping
            {
                Address = shippingDto.Address,
                City = shippingDto.City,
                PostalCode = shippingDto.PostalCode,
                ShippingFee = shippingDto.ShippingFee,
                EstimatedDays = shippingDto.EstimatedDays
            };

            var result = await _shippingService.UpdateShippingAsync(id, shipping);
            return Ok(result);
        }

        // DELETE: api/Shipping/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipping(int id)
        {
            var result = await _shippingService.DeleteShippingAsync(id);
            return Ok(result);
        }

        // GET: api/Shipping/paged?pageNow=1&pageSize=10
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedShippings([FromQuery] int pageNow = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _shippingService.GetPagedShippingsAsync(pageNow, pageSize);
            return Ok(result);
        }

        // GET: api/Shipping/search?address=xxx&city=yyy
        [HttpGet("search")]
        public async Task<IActionResult> SearchShippings([FromQuery] string? address, [FromQuery] string? city)
        {
            var result = await _shippingService.SearchShippingsAsync(address, city);
            return Ok(result);
        }

        // GET: api/Shipping/search/paged?address=xxx&city=yyy&pageNow=1&pageSize=10
        [HttpGet("search/paged")]
        public async Task<IActionResult> SearchShippingsWithPagination(
            [FromQuery] string? address,
            [FromQuery] string? city,
            [FromQuery] int pageNow = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _shippingService.SearchShippingsWithPaginationAsync(address, city, pageNow, pageSize);
            return Ok(result);
        }
    }
}
