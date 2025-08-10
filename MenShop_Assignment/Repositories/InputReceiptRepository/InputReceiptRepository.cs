using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Repositories.InputReceiptRepository;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.InputReceiptRepositories
{
    public class InputReceiptRepository : IInputReceiptRepository
    {
        private readonly ApplicationDbContext _context;
        public InputReceiptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<List<InputReceipt>> GetReceipts()
        {
            var list = await _context.InputReceipts
                .Include(i => i.InputReceiptDetails)
                    .ThenInclude(id => id.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                .Include(i => i.InputReceiptDetails)
                    .ThenInclude(id => id.ProductDetail)
                        .ThenInclude(pd => pd.Color)
                .Include(i => i.InputReceiptDetails)
                    .ThenInclude(id => id.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                .Include(i => i.InputReceiptDetails)
                    .ThenInclude(id => id.ProductDetail)
                        .ThenInclude(pd => pd.Fabric)
                .Include(i => i.Storage)
                    .ThenInclude(s => s.CategoryProduct)
                .Include(i => i.Manager).ToListAsync();
            return list;
        }

        public async Task<List<InputReceiptViewModel>?> GetInputReceipts()
        {
            var inputReceipts = GetReceipts().Result.ToList();
            return inputReceipts.Select(InputReceiptMapper.ToInputReceiptViewModel).ToList() ?? [];
        }
        public async Task<List<ProductDetailViewModel>> GetByIdAsync(int id)
        {
            var details = await _context.InputReceiptDetails
                .Where(x => x.ReceiptId == id)
                .Include(d => d.ProductDetail)
                    .ThenInclude(p => p.Product)
                .Include(d => d.ProductDetail)
                    .ThenInclude(p => p.Fabric)
                .Include(d => d.ProductDetail)
                    .ThenInclude(p => p.Color)
                .Include(d => d.ProductDetail)
                    .ThenInclude(p => p.Size)
                .Include(d => d.ProductDetail.Images)
                .ToListAsync();

            return details.Select(InputReceiptMapper.ToInputReceiptDetailViewModel).ToList();
        }



        public async Task<bool> ConfirmReceipt(int Id)
        {
            var inputReceipt = _context.InputReceipts.Where(x => x.ReceiptId == Id).Include(c => c.InputReceiptDetails).FirstOrDefault();
            if (inputReceipt == null)
                return false;

            inputReceipt.ConfirmedDate = DateTime.Now;
            inputReceipt.Status = Extensions.OrderStatus.Completed;
            if (inputReceipt.InputReceiptDetails == null)
                return false;

            foreach (var detail in inputReceipt.InputReceiptDetails)
            {
                var historyPrice = new HistoryPrice
                {
                    InputPrice = detail.Price,
                    ProductDetailId = detail.ProductDetailId,
                    UpdatedDate = DateTime.Now
                };
                _context.HistoryPrices.Add(historyPrice);

                var storageDetail = _context.StorageDetails
                    .FirstOrDefault(x => x.StorageId == inputReceipt.StorageId
                                      && x.ProductDetailId == detail.ProductDetailId
                                      && x.Price == detail.Price);

                if (storageDetail == null)
                {
                    var newStorageDetail = new StorageDetail
                    {
                        StorageId = inputReceipt.StorageId,
                        ProductDetailId = detail.ProductDetailId,
                        Price = detail.Price,
                        Quantity = detail.Quantity,
                    };
                    await _context.StorageDetails.AddAsync(newStorageDetail);
                }
                else
                {
                    storageDetail.Quantity += detail.Quantity;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateInputReceipt(List<CreateReceiptDetailDTO> detailDTOs, string ManagerId)
        {
            var groupedDetails = detailDTOs
                .Where(x => x.CategoryId.HasValue)
                .GroupBy(x => x.CategoryId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var kvp in groupedDetails)
            {
                int storageId = kvp.Key;
                var groupDetails = kvp.Value;

                var newReceipt = new InputReceipt
                {
                    CreatedDate = DateTime.Now,
                    ManagerId = ManagerId,
                    Status = OrderStatus.Created,
                    StorageId = storageId,
                    Total = 0,
                };

                await _context.InputReceipts.AddAsync(newReceipt);
                await _context.SaveChangesAsync(); // Lưu để có ReceiptId

                decimal total = 0;

                foreach (var dto in groupDetails)
                {
                    if (!dto.ProductDetailId.HasValue || !dto.Quantity.HasValue)
                        continue;

                    total += dto.Price.HasValue ? dto.Price.Value * dto.Quantity.Value : 0;

                    var detail = new InputReceiptDetail
                    {
                        ReceiptId = newReceipt.ReceiptId,
                        ProductDetailId = dto.ProductDetailId.Value,
                        Quantity = dto.Quantity.Value,
                        Price = dto.Price
                    };

                    await _context.InputReceiptDetails.AddAsync(detail);
                }

                newReceipt.Total = total;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelReceipt(int Id)
        {
            var receipt = await _context.InputReceipts.Where(x => x.ReceiptId == Id).FirstOrDefaultAsync();
            receipt.Status = Extensions.OrderStatus.Cancelled;
            receipt.CancelDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}