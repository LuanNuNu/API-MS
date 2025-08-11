using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CollectionRepository;
using MenShop_Assignment.Repositories.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionRepository _repo;

        public CollectionController(ICollectionRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllCollection();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-current-collection")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentCollection()
        {
            var result = await _repo.GetCurrentCollectionAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repo.GetByIdCollection(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CreateCollection")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> AddCollection([FromBody] CollectionCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CollectionName) || dto.StartTime >= dto.EndTime)
            {
                return BadRequest(new ApiResponseModel<string>(false, "Dữ liệu không hợp lệ", null, 400));
            }

            var collection = new Collection
            {
                CollectionName = dto.CollectionName,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = dto.Status
            };

            var result = await _repo.AddCollection(collection);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-collection/{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> UpdateCollection(int id, [FromBody] CollectionCreateDTO dto)
        {
            var updated = new Collection
            {
                CollectionId = id,
                CollectionName = dto.CollectionName,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = dto.Status
            };

            var result = await _repo.UpdateCollection(updated);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-collection/{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> DeleteCollection(int id)
        {
            var result = await _repo.DeleteCollection(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{collectionId}/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetails(int collectionId)
        {
            var result = await _repo.GetCollectionDetailsByCollectionId(collectionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-details")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> AddDetail([FromBody] CollectionDetailCreateDTO dto)
        {
            var detail = new CollectionDetail
            {
                CollectionId = dto.CollectionId,
                ProductId = dto.ProductId
            };

            var result = await _repo.AddDetail(detail);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("details/{detailId}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> UpdateDetail(int detailId, [FromBody] CollectionDetailCreateDTO dto)
        {
            var detail = new CollectionDetail
            {
                CollectionDetailId = detailId,
                CollectionId = dto.CollectionId,
                ProductId = dto.ProductId
            };

            var result = await _repo.UpdateDetail(detail);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("details/{detailId}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            var result = await _repo.DeleteDetail(detailId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _repo.UpdateCollectionStatus(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-images/{collectionId}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> AddImages(int collectionId, [FromBody] List<string> imageUrls)
        {
            var result = await _repo.AddImagesToCollectionAsync(collectionId, imageUrls);
            if (result.Count == 0 || result.Any(r => !r.IsSuccess))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("collection/images/{imgId}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> DeleteImgProductDetail(int imgId)
        {
            try
            {
                await _repo.DeleteImageAsync(imgId);
                return Ok("Xóa ảnh chi tiết sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("collection/get-images/{collectionId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetImgProductDetails(int collectionId)
        {
            var productImgDetails = await _repo.GetImgByCollectionIdAsync(collectionId);
            return Ok(productImgDetails);
        }
    }

}