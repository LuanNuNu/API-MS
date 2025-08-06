using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.OutputReceiptRepositories
{
    public class OutputReceiptRepository : IOutputReceiptRepository
    {
        private readonly ApplicationDbContext _context;
        public OutputReceiptRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        private async Task<List<OutputReceipt>> GetReceipts()
        {
            var list = await _context.OutputReceipts
                .Include(o => o.OutputReceiptDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                .Include(o => o.OutputReceiptDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Color)
                .Include(o => o.OutputReceiptDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                .Include(o => o.OutputReceiptDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Fabric)
                .Include(o => o.Branch)
                .Include(o => o.Manager).ToListAsync();
            return list;
        }
        public async Task<List<OutputReceiptViewModel>> GetOutputReceiptViews()
        {
            var orderList = await GetReceipts();
            return orderList.Select(OutputReceiptMapper.ToOutReceiptView).ToList();
        }
        public async Task<List<ProductDetailViewModel>> GetById(int id)
        {
            var receipts = await GetReceipts();
            var receipt = receipts.FirstOrDefault(x => x.ReceiptId == id);

            if (receipt == null || receipt.OutputReceiptDetails == null)
                return new List<ProductDetailViewModel>();
            var result = receipt.OutputReceiptDetails
                                .Select(OutputReceiptMapper.ToOutputReceiptDetailView)
                                .ToList();

            return result;
        }
        public async Task<List<OutputReceiptViewModel>> GetByBranchId(int BranchId)
        {
            var outputReceipts = await _context.OutputReceipts.Where(x => x.BranchId == BranchId).ToListAsync();
            return outputReceipts.Select(OutputReceiptMapper.ToOutReceiptView).ToList();
        }
        public async Task<bool> ConfirmReceipt(int Id)
        {
            var outputReceipt = _context.OutputReceipts.Where(x => x.ReceiptId == Id).Include(c => c.OutputReceiptDetails).FirstOrDefault();

            if (outputReceipt == null)
                return false;

            outputReceipt.ConfirmedDate = DateTime.Now;
            outputReceipt.Status = Extensions.OrderStatus.Completed;

            if (outputReceipt.OutputReceiptDetails == null)
                return false;

            foreach (var detail in outputReceipt.OutputReceiptDetails)
            {
                var historyPrice = new HistoryPrice
                {
                    SellPrice = detail.Price,
                    ProductDetailId = detail.ProductDetailId,
                    UpdatedDate = DateTime.Now
                };
                _context.HistoryPrices.Add(historyPrice);
                await _context.SaveChangesAsync();
                var branchDetail = _context.BranchDetails.Where(x => x.BranchId == outputReceipt.BranchId && x.ProductDetailId == detail.ProductDetailId).FirstOrDefault();
                if (branchDetail == null)
                {
                    var newBranchDetail = new BranchDetail
                    {
                        BranchId = outputReceipt.BranchId,
                        ProductDetailId = detail.ProductDetailId,
                        Price = detail.Price,
                        Quantity = detail.Quantity,
                    };
                    _context.BranchDetails.Add(newBranchDetail);
                    await _context.SaveChangesAsync();
                    continue;
                }
                branchDetail.Price = detail.Price;
                branchDetail.Quantity += detail.Quantity;
                await _context.SaveChangesAsync();
            }
            return true;
        }
        public async Task<bool> CreateReceipt(int branchId, string managerId, List<CreateReceiptDetailDTO> detailDTOs)
        {
            var outputDetails = detailDTOs.Select(x => OutputReceiptMapper.ToOutputReceiptDetail(x, _context)).ToList();
            var newOutputReceipt = new OutputReceipt
            {
                BranchId = branchId,
                CreatedDate = DateTime.Now,
                ManagerId = managerId,
                Status = Extensions.OrderStatus.Pending,
                Total = 0,
            };
            await _context.OutputReceipts.AddAsync(newOutputReceipt);
            await _context.SaveChangesAsync();
            decimal total = 0;
            foreach (var detail in outputDetails)
            {
                detail.ReceiptId = newOutputReceipt.ReceiptId;
                detail.OutputReceipt = newOutputReceipt;
                detail.Price = _context.StorageDetails.Where(x => x.ProductDetailId == detail.ProductDetailId).FirstOrDefault().Price / 100 * 120;
                total += (decimal)detail.Price;
                StorageDetail storageDetail = _context.StorageDetails.Where(x => x.ProductDetailId == detail.ProductDetailId).FirstOrDefault();
                storageDetail.Quantity -= detail.Quantity;
                await _context.OutputReceiptDetails.AddAsync(detail);
            }
            newOutputReceipt.Total = total;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelReceipt(int Id)
        {
            var receipt = await _context.OutputReceipts.Where(x => x.ReceiptId == Id).Include(o => o.OutputReceiptDetails).FirstOrDefaultAsync();
            if (receipt.Status != OrderStatus.Pending)
                return false;
            foreach (var detail in receipt.OutputReceiptDetails)
            {
                var storagedetail = _context.StorageDetails.Where(x => x.ProductDetailId == detail.ProductDetailId).FirstOrDefault();
                storagedetail.Quantity += detail.Quantity;
                await _context.SaveChangesAsync();
            }
            receipt.Status = Extensions.OrderStatus.Cancelled;
            receipt.CancelDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}