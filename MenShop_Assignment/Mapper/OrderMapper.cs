using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class OrderMapper
    {
        public static OrderViewModel ToOrderViewModel(Order order)
        {
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.UserName ?? null,
                EmployeeName = order.Employee?.UserName ?? null,
                ShipperName = order.Shipper?.UserName ?? null,
                CreatedDate = order.CreatedDate ?? null,
                CompletedDate = order.CompletedDate ?? null,
                PaidDate = order.PaidDate ?? null,
                Status = order.Status.ToString() ?? null,
				IsOnline = (order.IsOnline == true ? "Online" : "Offline") ?? null,
                Total = order.Total ?? null,
                Details = order.Details?.Select(ToOrderDetailViewModel).ToList() ?? [],
                Payments = order.Payments?.Select(PaymentMapper.ToPaymentViewModel).ToList() ?? []
            };
        }
        public static ProductDetailViewModel ToOrderDetailViewModel(OrderDetail orderDetail)
        {
            return new ProductDetailViewModel
			{
                DetailId = orderDetail.ProductDetailId,
                ProductName = orderDetail.ProductDetail?.Product?.ProductName ?? null,
                SizeName = orderDetail.ProductDetail?.Size?.Name ?? null,
                ColorName = orderDetail.ProductDetail?.Color?.Name ?? null,
				FabricName = orderDetail.ProductDetail?.Fabric?.Name ?? null,
                Quantity = orderDetail.Quantity ?? null,
                SellPrice = orderDetail.Price ?? null,
            };
        }
    }

}
