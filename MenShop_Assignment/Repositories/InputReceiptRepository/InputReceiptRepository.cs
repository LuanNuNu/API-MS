using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.InputReceiptRepository
{
	public class InputReceiptRepository : IInputReceiptRepository
	{
		private readonly ApplicationDbContext _context;
		public InputReceiptRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<List<InputReceiptViewModel>?> GetInputReceipts()
		{
			var inputReceipts = await _context.InputReceipts
				.Include(i=>i.Storage)
					.ThenInclude(s=>s.CategoryProduct)	
				.Include(i=>i.Manager).ToListAsync();
			return inputReceipts.Select(InputReceiptMapper.ToInputReceiptViewModel).ToList() ?? [];
		}
		public async Task<InputReceiptViewModel?> GetByIdAsync(int Id)
		{
			var receipt = await _context.InputReceipts.Include(i => i.InputReceiptDetails).Include(i => i.Storage).Include(i => i.Manager).Where(x => x.ReceiptId == Id).FirstOrDefaultAsync();
			return InputReceiptMapper.ToInputReceiptViewModel(receipt) ?? null;
		}
		public async Task<bool> ConfirmReceipt(int Id)
		{
			var inputReceipt = _context.InputReceipts.Where(x => x.ReceiptId == Id).Include(c => c.InputReceiptDetails).FirstOrDefault();
			if (inputReceipt == null)
				return false;

			inputReceipt.ConfirmedDate = DateTime.UtcNow;
			inputReceipt.Status = Extensions.OrderStatus.Completed;
			if (inputReceipt.InputReceiptDetails == null)
				return false;

			foreach (var detail in inputReceipt.InputReceiptDetails)
			{
				var historyPrice = new HistoryPrice
				{
					InputPrice = detail.Price,
					ProductDetailId = detail.ProductDetailId,
					UpdatedDate = DateTime.UtcNow
				};
				_context.HistoryPrices.Add(historyPrice);
				await _context.SaveChangesAsync();

				var storageDetail = _context.StorageDetails.Where(x => x.StorageId == inputReceipt.StorageId && x.ProductDetailId == detail.ProductDetailId).FirstOrDefault();
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
					continue;
				}
				storageDetail.Price = detail.Price;
				storageDetail.Quantity += detail.Quantity;
				_context.SaveChanges();
			}
			return true;
		}
		public async Task<bool> CreateInputReceipt(List<CreateReceiptDetailDTO> detailDTOs, string ManagerId)
		{
			List<InputReceiptDetail> inputReceiptDetails = detailDTOs.Select(x => InputReceiptMapper.ToInputReceiptDetail(x, _context)).ToList();
			var productDetailIds = inputReceiptDetails.Select(ird => ird.ProductDetailId).ToList();
			var uniqueCategoryId = _context.ProductDetails.Where(pd => productDetailIds.Contains(pd.DetailId))
				.Include(pd => pd.Product)
				.Select(pd => pd.Product.CategoryId.Value)
				.Distinct()
				.ToList();

			for (int i = 0; i < uniqueCategoryId.Count; i++)
			{
				var newReceipt = new InputReceipt
				{
					ConfirmedDate = DateTime.Now,
					ManagerId = ManagerId,
					Status = Extensions.OrderStatus.Created,
					StorageId = _context.Storages.Where(x => x.CategoryId == uniqueCategoryId[i]).FirstOrDefault().StorageId,
					Total = 0,
				};
				_context.InputReceipts.Add(newReceipt);
				await _context.SaveChangesAsync();
				decimal total = 0;
				foreach (var detail in inputReceiptDetails)
				{
					total += detail.Price.Value;
					var productDetail = await _context.ProductDetails.Where(x => x.DetailId == detail.ProductDetailId).Include(x => x.Product).FirstOrDefaultAsync();
					if (productDetail.Product.CategoryId == uniqueCategoryId[i])
					{
						var detailCopy = new InputReceiptDetail
						{
							ReceiptId = newReceipt.ReceiptId,
							ProductDetailId = detail.ProductDetailId,
							Quantity = detail.Quantity,
							Price = detail.Price
						};
						_context.InputReceiptDetails.Add(detailCopy);
					}
				}
				newReceipt.Total = total;
				await _context.SaveChangesAsync();
			}
			return true;
		}
		public async Task<bool> CancelReceipt(int Id)
		{
			var receipt = await _context.InputReceipts.Where(x => x.ReceiptId == Id).FirstOrDefaultAsync();
			if (receipt.Status != Extensions.OrderStatus.Pending)
			{
				receipt.Status = Extensions.OrderStatus.Cancelled;
				await _context.SaveChangesAsync();
			}
			return true;
		}
	}
}
