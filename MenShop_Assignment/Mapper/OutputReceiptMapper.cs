using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class OutputReceiptMapper
    {
        
        public static OutputReceiptViewModel ToOutReceiptView(OutputReceipt outputReceipt)
        {
            return new OutputReceiptViewModel
			{
				ReceiptId = outputReceipt.ReceiptId,
				CreatedDate = outputReceipt.CreatedDate ?? null,
				CancelDate = outputReceipt.CancelDate ?? null,
				ConfirmedDate = outputReceipt.ConfirmedDate ?? null,
				Status = outputReceipt.Status.ToString() ?? null,
				Total = outputReceipt.Total ?? null,
				BranchName = outputReceipt.Branch !=null ? "Chi nhánh " + outputReceipt.Branch.Address : null,
				ManagerName = outputReceipt.Manager?.FullName ?? null,
				OutputReceiptDetails = outputReceipt.OutputReceiptDetails?.Select(ToOutputReceiptDetailView).ToList() ?? [],
            };
        }

        public static ProductDetailViewModel ToOutputReceiptDetailView(OutputReceiptDetail receiptDetail)
        {
            return new ProductDetailViewModel
			{
				DetailId = receiptDetail.ProductDetailId,
				ProductName = receiptDetail.ProductDetail?.Product?.ProductName ?? null,
				SizeName = receiptDetail.ProductDetail?.Size?.Name ?? null,
				ColorName = receiptDetail.ProductDetail?.Color?.Name ?? null,
				FabricName = receiptDetail.ProductDetail?.Fabric?.Name ?? null,
				Quantity = receiptDetail.Quantity ?? null,
				SellPrice = receiptDetail.Price ?? null,
				Images = receiptDetail.ProductDetail?.Images?.Select(x => x.FullPath).ToList() ?? [],
			};
        }
		public static OutputReceiptDetail ToOutputReceiptDetail(CreateReceiptDetailDTO dto, ApplicationDbContext context)
		{


			var productDetail = context.ProductDetails.FirstOrDefault(pd =>
				pd.ProductId == dto.ProductId &&
				pd.ColorId == dto.ColorId &&
				pd.SizeId == dto.SizeId &&
				pd.FabricId == dto.FabricId);


			return new OutputReceiptDetail
			{
				ProductDetailId = productDetail.DetailId,
				Quantity = dto.Quantity,
			};
		}
	}
}
