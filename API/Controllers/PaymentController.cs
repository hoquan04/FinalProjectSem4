using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => route mặc định: api/payment
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _paymentRepository.GetAllPaymentsAsync();
            return Ok(response);
        }

        // GET: api/payment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _paymentRepository.GetPaymentByIdAsync(id);

            if (!response.Success || response.Data == null)
                return NotFound(response);

            return Ok(response);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var model = new Payment
            {
                OrderId = dto.OrderId,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending, // mặc định
                CreatedAt = DateTime.Now
            };

            var response = await _paymentRepository.CreatePaymentAsync(model);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data?.PaymentId }, response);
        }

        // PUT: api/payment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var model = new Payment
            {
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus,
                PaidAt = dto.PaidAt
            };

            var response = await _paymentRepository.UpdatePaymentAsync(id, model);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // DELETE: api/payment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _paymentRepository.DeletePaymentAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // GET: api/payment/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(int pageNow = 1, int pageSize = 10)
        {
            var response = await _paymentRepository.GetPageAsync(pageNow, pageSize);
            return Ok(response);
        }

        // GET: api/payment/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { Success = false, Message = "Keyword không được để trống" });

            var response = await _paymentRepository.SearchPaymentsAsync(keyword);
            return Ok(response);
        }



    }
}
