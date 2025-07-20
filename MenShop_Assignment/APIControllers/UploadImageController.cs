using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        //up1
        [HttpPost]
        public IActionResult UploadFiles()
        {
            try
            {
                var files = Request.Form.Files;
                if (files == null || files.Count == 0)
                {
                    return BadRequest("Chưa có tệp nào được tải lên.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var uploadedFilePaths = new List<string>();

                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest("Một hoặc nhiều tệp có định dạng không hợp lệ.");
                    }

                    var originalFileName = Path.GetFileName(file.FileName);
                    var fileName = originalFileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    uploadedFilePaths.Add(dbPath);
                }

                return Ok(new { FilePaths = uploadedFilePaths });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi tải lên tệp: {ex.Message}");
            }
        }

    }
}

