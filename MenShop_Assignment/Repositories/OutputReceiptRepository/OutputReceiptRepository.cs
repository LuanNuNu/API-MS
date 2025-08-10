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
            // Tìm phiếu xuất và load chi tiết
            var outputReceipt = await _context.OutputReceipts
                .Where(x => x.ReceiptId == Id)
                .Include(c => c.OutputReceiptDetails)
                .FirstOrDefaultAsync();

            if (outputReceipt == null || outputReceipt.OutputReceiptDetails == null)
                return false;


            outputReceipt.ConfirmedDate = DateTime.Now;
            outputReceipt.Status = OrderStatus.Completed;

            foreach (var detail in outputReceipt.OutputReceiptDetails)
            {
                var latestHistory = await _context.HistoryPrices
                    .Where(h => h.ProductDetailId == detail.ProductDetailId)
                    .OrderByDescending(h => h.UpdatedDate)
                    .FirstOrDefaultAsync();

                if (latestHistory != null && latestHistory.SellPrice == null)
                {
                    latestHistory.SellPrice = detail.Price;
                    latestHistory.UpdatedDate = DateTime.Now;
                }
                else
                {
                    var historyPrice = new HistoryPrice
                    {
                        SellPrice = detail.Price,
                        ProductDetailId = detail.ProductDetailId,
                        UpdatedDate = DateTime.Now
                    };
                    _context.HistoryPrices.Add(historyPrice);
                }

                var branchDetail = await _context.BranchDetails
                    .FirstOrDefaultAsync(x => x.BranchId == outputReceipt.BranchId && x.ProductDetailId == detail.ProductDetailId);

                if (branchDetail == null)
                {
                    var newBranchDetail = new BranchDetail
                    {
                        BranchId = outputReceipt.BranchId,
                        ProductDetailId = detail.ProductDetailId,
                        Price = detail.Price,
                        Quantity = detail.Quantity
                    };
                    _context.BranchDetails.Add(newBranchDetail);
                }
                else
                {
                    if (branchDetail.Price != detail.Price)
                    {
                        branchDetail.Price = detail.Price;
                    }

                    branchDetail.Quantity += detail.Quantity;
                }
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateReceipt(int branchId, string managerId, List<CreateReceiptDetailDTO> detailDTOs)
        {
            var outputDetails = detailDTOs.Select(x => OutputReceiptMapper.ToOutputReceiptDetail(x)).ToList();

            var newOutputReceipt = new OutputReceipt
            {
                BranchId = branchId,
                CreatedDate = DateTime.Now,
                ManagerId = managerId,
                Status = OrderStatus.Pending,
                Total = 0,
            };

            await _context.OutputReceipts.AddAsync(newOutputReceipt);
            await _context.SaveChangesAsync();

            decimal total = 0;

            for (int i = 0; i < outputDetails.Count; i++)
            {
                var detail = outputDetails[i];
                var dto = detailDTOs[i]; 

                detail.ReceiptId = newOutputReceipt.ReceiptId;
                detail.OutputReceipt = newOutputReceipt;

                var storageDetail = await _context.StorageDetails.FirstOrDefaultAsync(x => x.ProductDetailId == detail.ProductDetailId);
                if (storageDetail == null)
                    continue;

                int profitPercent = dto.ProfitPercent ?? 0;

                detail.Price = storageDetail.Price + (storageDetail.Price * profitPercent / 100);
                total += (decimal)(detail.Price * detail.Quantity);

                storageDetail.Quantity -= detail.Quantity ?? 0;

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