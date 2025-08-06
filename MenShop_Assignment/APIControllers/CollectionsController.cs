using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CollectionRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly ICollectionRepository _repo;

        public CollectionsController(ICollectionRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllCollection();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repo.GetByIdCollection(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CreateCollection")]
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
        public async Task<IActionResult> DeleteCollection(int id)
        {
            var result = await _repo.DeleteCollection(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{collectionId}/details")]
        public async Task<IActionResult> GetDetails(int collectionId)
        {
            var result = await _repo.GetCollectionDetailsByCollectionId(collectionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-details")]
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
        public async Task<IActionResult> DeleteDetail(int detailId)
        {
            var result = await _repo.DeleteDetail(detailId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var result = await _repo.UpdateCollectionStatus(id, dto.Status);
            return StatusCode(result.StatusCode, result);
        }
    }

}