namespace AdminWeb.Areas.Admin.Data
{
    public class Upload
    {
        public async Task<string> UploadFile(IFormFile file, string subFolder = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/" + Path.Combine(subFolder, fileName).Replace("\\", "/");
        }

    }
}
