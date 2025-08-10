using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Mapper
{
    public static class InputReceiptMapper
    {
        public static InputReceiptViewModel ToInputReceiptViewModel(InputReceipt inputReceipt)
        {
            return new InputReceiptViewModel
            {
                ReceiptId = inputReceipt.ReceiptId,
                CreatedDate = inputReceipt.CreatedDate ?? null,
                ConfirmedDate = inputReceipt.ConfirmedDate ?? null,
                CancelDate = inputReceipt.CancelDate ?? null,
                ManagerName = inputReceipt.Manager?.FullName ?? null,

                StorageName = "Kho " + inputReceipt.Storage?.CategoryProduct?.Name ?? null,
                Status = inputReceipt.Status.ToString() ?? null,
                Total = inputReceipt.Total ?? 0,
                InputReceiptDetails = inputReceipt.InputReceiptDetails?.Select(ToInputReceiptDetailViewModel).ToList() ?? [],
            };
        }
        public static ProductDetailViewModel ToInputReceiptDetailViewModel(InputReceiptDetail inputReceiptDetail)
        {
            return new ProductDetailViewModel
            {
                DetailId = inputReceiptDetail.ProductDetailId,
                ProductName = inputReceiptDetail.ProductDetail?.Product?.ProductName ?? null,
                FabricName = inputReceiptDetail.ProductDetail?.Fabric?.Name ?? null,
                ColorName = inputReceiptDetail.ProductDetail?.Color?.Name ?? null,
                SizeName = inputReceiptDetail.ProductDetail?.Size?.Name ?? null,
                InputPrice = inputReceiptDetail.Price ?? 0,
                Images = inputReceiptDetail.ProductDetail?.Images?.Select(x => x.FullPath).ToList() ?? [],
                Quantity = inputReceiptDetail.Quantity ?? 0,
            };
        }

        public static InputReceiptDetail ToInputReceiptDetail(CreateReceiptDetailDTO dto)
        {
            return new InputReceiptDetail
            {
                ProductDetailId = dto.ProductDetailId ?? 0,
                Quantity = dto.Quantity,
                Price = dto.Price
            };
        }
    }
}