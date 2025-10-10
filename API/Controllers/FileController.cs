using Microsoft.AspNetCore.Mvc;
using API.Repositories.RestAPI;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var response = new APIRespone<string>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "Không có file được tải lên";
                    return BadRequest(response);
                }

                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    response.Success = false;
                    response.Message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)";
                    return BadRequest(response);
                }

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    response.Success = false;
                    response.Message = "File không được vượt quá 5MB";
                    return BadRequest(response);
                }

                // Tạo thư mục uploads nếu chưa có
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL của file
                var fileUrl = $"/uploads/products/{fileName}";
                
                response.Success = true;
                response.Message = "Upload file thành công";
                response.Data = fileUrl;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi upload file: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromQuery] string fileName, [FromQuery] string folder = "products")
        {
            var response = new APIRespone<bool>();

            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    response.Success = false;
                    response.Message = "Tên file không được để trống";
                    return BadRequest(response);
                }

                var safeFileName = Path.GetFileName(fileName);
                var uploadsFolder = folder == "cccd" ? "cccd" : "products";
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", uploadsFolder, safeFileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    response.Success = true;
                    response.Message = "Xóa file thành công";
                    response.Data = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "File không tồn tại";
                    response.Data = false;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi xóa file: {ex.Message}";
                response.Data = false;
                return StatusCode(500, response);
            }
        }


        [HttpPost("upload-cccd")]
        public async Task<IActionResult> UploadCCCD(IFormFile file)
        {
            var response = new APIRespone<string>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "Không có file được tải lên";
                    return BadRequest(response);
                }

                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    response.Success = false;
                    response.Message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)";
                    return BadRequest(response);
                }

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    response.Success = false;
                    response.Message = "File không được vượt quá 5MB";
                    return BadRequest(response);
                }

                // 📁 Lưu riêng thư mục /uploads/cccd/
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "cccd");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL công khai
                var fileUrl = $"/uploads/cccd/{fileName}";

                response.Success = true;
                response.Message = "Upload ảnh CCCD thành công";
                response.Data = fileUrl;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi upload ảnh CCCD: {ex.Message}";
                return StatusCode(500, response);
            }
        }

    }
}
