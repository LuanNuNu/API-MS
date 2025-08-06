using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class OrderMapper
    {
        //up1
        public static OrderViewModel ToOrderViewModel(Order order)
        {
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.FullName?? null,
                EmployeeName = order.Employee?.FullName ?? null,
                ShipperName = order.Shipper?.FullName ?? null,
                ShipperId = order.Shipper?.Id,
                BranchName = order.Branch?.Name ?? null,
                CreatedDate = order.CreatedDate ?? null,
                CompletedDate = order.CompletedDate ?? null,
                PaidDate = order.PaidDate ?? null,
                Status = order.Status,
                IsOnline = (order.IsOnline == true ? "Online" : "Offline") ?? null,
                Subtotal= order.Subtotal ?? null,
                ShippingFee= order.ShippingFee ?? null,
                Address = order.Address,
                ReceiverName = order.ReceiverName ?? null,
                ReceiverPhone = order.ReceiverPhone ?? null,
                BranchId = order.BranchId ?? null,
                CancellationReason = order.CancellationReason ?? null
            };
        }
        public static OrderProductDetailViewModel ToOrderDetailViewModel(OrderDetail orderDetail, decimal? shippingFee)
        {
            return new OrderProductDetailViewModel
            {
                DetailId = orderDetail.ProductDetailId,
                ProductName = orderDetail.ProductDetail?.Product?.ProductName ?? null,
                SizeName = orderDetail.ProductDetail?.Size?.Name ?? null,
                ColorName = orderDetail.ProductDetail?.Color?.Name ?? null,
				FabricName = orderDetail.ProductDetail?.Fabric?.Name ?? null,
                Quantity = orderDetail.Quantity ?? null,
                SellPrice = orderDetail.Price ?? null,
                ShippingFee = shippingFee
            };
        }
    }

}
