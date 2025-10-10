using API.Data;
using API.Models.DTOs;
using API.Repositories;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutRepository _checkoutRepo;

        public CheckoutController(CheckoutRepository _checkoutRepository)
        {
            this._checkoutRepo = _checkoutRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
        {
            var result = await _checkoutRepo.CheckoutAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

    }
}
