using Azure;
using MenShop_Assignment.Datas;
using MenShop_Assignment.Services;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CartRepository;
using MenShop_Assignment.Services.PaymentServices;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;


        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        private IQueryable<Order> GetOrderQuery()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Shipper)
                .Include(o => o.Branch)
                .AsNoTracking();
        }
        private IQueryable<OrderDetail> GetOrderDetailQuery()
        {
            return _context.OrderDetails
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Product)
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Size)
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Color)
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Fabric)
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Images)
                .AsNoTracking();
        }


        public async Task<List<OrderViewModel>> GetOrdersAsync(SearchOrderDTO? search)
        {
            try
            {
                var query = GetOrderQuery();
                var validDistricts = new[] { "Liên Chiểu", "Hải Châu", "Ngũ Hành Sơn", "Sơn Trà", "Cẩm Lệ", "Thanh Khê" };

                if (search != null)
                {
                    if (!string.IsNullOrWhiteSpace(search.CustomerId))
                        query = query.Where(x => x.CustomerId == search.CustomerId);

                    if (!string.IsNullOrWhiteSpace(search.OrderId))
                        query = query.Where(x => x.OrderId == search.OrderId);

                    if (!string.IsNullOrWhiteSpace(search.ShipperId))
                        query = query.Where(x => x.ShipperId == search.ShipperId);

                    if (search.Status.HasValue)
                        query = query.Where(x => x.Status == search.Status);

                    if (!string.IsNullOrWhiteSpace(search.District))
                    {
                        if (!validDistricts.Contains(search.District))
                            throw new ArgumentException($"Quận/huyện không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validDistricts)}");

                        query = query
                            .AsEnumerable()
                            .Where(x => x.Address != null &&
                                        x.Address.Contains(search.District, StringComparison.OrdinalIgnoreCase))
                            .AsQueryable();
                    }

                    if (search.StartDate.HasValue && search.EndDate.HasValue)
                    {
                        query = query.Where(x => x.CompletedDate.HasValue &&
                                                 x.CompletedDate >= search.StartDate.Value &&
                                                 x.CompletedDate <= search.EndDate.Value);
                    }

                    if (search.IsOnline.HasValue)
                    {
                        query = query.Where(x => x.IsOnline == search.IsOnline);
                    }
                }

                var orderList = query.ToList(); 
                return orderList.Select(OrderMapper.ToOrderViewModel).ToList();
            }
            catch (ArgumentException argEx)
            {
                // lỗi do nhập sai quận
                throw new ApplicationException("Lỗi tìm kiếm: " + argEx.Message);
            }
            catch (Exception ex)
            {
                // lỗi bất ngờ khác
                throw new ApplicationException("Đã xảy ra lỗi khi lấy danh sách đơn hàng. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task<List<OrderProductDetailViewModel>> GetOrderDetailsByOrderIdsAsync(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return new List<OrderProductDetailViewModel>();

            var shippingFee = order.ShippingFee;
            var details = await GetOrderDetailQuery()
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return details.Select(od => OrderMapper.ToOrderDetailViewModel(od, shippingFee)).ToList();
        }

        public async Task<ApiResponseModel<object>> ShipperAcceptOrderByOrderId(string orderId, string shipperId)
        {
            var order = await GetOrderQuery().FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
            {
                return new ApiResponseModel<object>(false, "Không tìm thấy đơn hàng", null, 404);
            }

            order.ShipperId = shipperId;
            order.Status = OrderStatus.Delivering;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ApiResponseModel<object>(true, "Xác nhận đơn hàng thành công!", null, 200);
        }
        public async Task<ApiResponseModel<object>> CompletedOrderStatus(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
            {
                return new ApiResponseModel<object>(false, "Không tìm thấy đơn hàng", null, 404);
            }

            order.Status = OrderStatus.Completed;
            order.CompletedDate = DateTime.Now;

            if (order.Payments != null && order.Payments.Any())
            {
                var codPayment = order.Payments.FirstOrDefault(p => p.Method == PaymentMethod.COD);

                if (codPayment != null)
                {
                    codPayment.Status = PaymentStatus.Completed;
                    codPayment.PaymentDate = DateTime.Now;
                    order.PaidDate = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            return new ApiResponseModel<object>(true, "Cập nhật trạng thái thành công", null, 200);
        }



        public async Task<ApiResponseModel<object>> CancelOrderAsync(string orderId, string reason)
        {
            try
            {
                var order = await _context.Orders
                                          .Include(o => o.Details)
                                          .AsSplitQuery()
                                          .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return new ApiResponseModel<object>(
                        false,
                        "Không tìm thấy đơn hàng.",
                        null,
                        404
                    );
                }

                if (!(order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed))
                {
                    return new ApiResponseModel<object>(
                        false,
                        "Chỉ có thể hủy đơn hàng ở trạng thái 'Chờ xác nhận' hoặc 'Đã xác nhận'!!",
                        null,
                        400
                    );
                }

                if (order.Status == OrderStatus.Confirmed && order.Details != null && order.Details.Any())
                {
                    var result = await RestoreStockForCancelledOrderAsync(orderId);
                    if (!result.Success)
                    {
                        return new ApiResponseModel<object>(
                            false,
                            "Hủy đơn hàng thất bại khi khôi phục kho.",
                            null,
                            500
                        );
                    }
                }
                order.CancellationReason = reason;
                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();

                return new ApiResponseModel<object>(
                    true,
                    "Hủy đơn hàng thành công.",
                    null,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<object>(
                    false,
                    "Lỗi xử lý khi hủy đơn hàng.",
                    null,
                    500,
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ApprovalResultDto> RestoreStockForCancelledOrderAsync(string orderId)
        {
            var orderEntity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (orderEntity == null)
            {
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng" };
            }

            if (orderEntity.Status != OrderStatus.Confirmed)
            {
                return new ApprovalResultDto
                {
                    Success = false,
                    OrderId = orderId,
                    Message = "Chỉ có thể hoàn tồn với đơn hàng đã xác nhận"
                };
            }
            var branchOnline = await _context.Branches.FirstOrDefaultAsync(b => b.IsOnline == true);
            if (branchOnline == null)
            {
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy chi nhánh online" };
            }

            var orderDetails = await GetOrderDetailsByOrderIdsAsync(orderId);

            foreach (var detail in orderDetails)
            {
                var branchStorage = await _context.BranchDetails
                    .FirstOrDefaultAsync(x => x.ProductDetailId == detail.DetailId && x.BranchId == branchOnline.BranchId);

                if (branchStorage == null)
                {
                    return new ApprovalResultDto
                    {
                        Success = false,
                        OrderId = orderId,
                        Message = $"Không tồn tại sản phẩm mã {detail.DetailId} trong tồn kho chi nhánh"
                    };
                }

                branchStorage.Quantity += detail.Quantity; 
                _context.Update(branchStorage);
            }

            orderEntity.Status = OrderStatus.Cancelled;
            _context.Update(orderEntity);

            await _context.SaveChangesAsync();

            return new ApprovalResultDto
            {
                Success = true,
                OrderId = orderId,
                Message = "Hoàn tồn thành công và cập nhật trạng thái đơn hàng thành 'Đã Hủy'"
            };
        }

        public async Task<ApprovalResultDto> DeductStockForOfflineOrderAsync(string orderId)
        {
            var orderEntity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (orderEntity == null)
            {
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng" };
            }

            if (orderEntity.Status != OrderStatus.Pending) 
            {
                return new ApprovalResultDto
                {
                    Success = false,
                    OrderId = orderId,
                    Message = "Chỉ xử lý trừ tồn cho đơn hàng chưa xử lý"
                };
            }

            var orderDetails = await GetOrderDetailsByOrderIdsAsync(orderId);

            foreach (var detail in orderDetails)
            {
                var branchStorage = await _context.BranchDetails
                    .FirstOrDefaultAsync(x => x.ProductDetailId == detail.DetailId && x.BranchId == orderEntity.BranchId);

                if (branchStorage == null || branchStorage.Quantity < detail.Quantity)
                {
                    return new ApprovalResultDto
                    {
                        Success = false,
                        OrderId = orderId,
                        Message = $"Không đủ hàng tồn hoặc không tìm thấy sản phẩm mã {detail.DetailId} trong chi nhánh"
                    };
                }

                branchStorage.Quantity -= detail.Quantity;
                _context.Update(branchStorage);
            }

            orderEntity.Status = OrderStatus.Confirmed; 
            _context.Update(orderEntity);

            await _context.SaveChangesAsync();

            return new ApprovalResultDto
            {
                Success = true,
                OrderId = orderId,
                Message = "Trừ tồn kho thành công và cập nhật trạng thái đơn hàng"
            };
        }

        //up
        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            var productDetailIds = createOrderDTO.Details.Select(d => d.ProductDetailId).ToList();
            var orderDetails = createOrderDTO.Details.Select(d => new OrderDetail
            {
                ProductDetailId = d.ProductDetailId,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToList();

            bool isOnline = string.IsNullOrWhiteSpace(createOrderDTO.EmployeeId);

            var order = new Order
            {
                OrderId = "OD" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                CustomerId = createOrderDTO.CustomerId,
                EmployeeId = isOnline ? null : createOrderDTO.EmployeeId,
                ShipperId = createOrderDTO.ShipperId,
                IsOnline = isOnline,
                BranchId = createOrderDTO.BranchId,
                CreatedDate = DateTime.Now,
                Status = OrderStatus.Pending,
                Subtotal = orderDetails.Sum(d => (d.Price ?? 0) * d.Quantity).GetValueOrDefault(),
                ShippingFee = createOrderDTO.ShippingFee ?? 0,
                Address = createOrderDTO.Address,
                ReceiverName = createOrderDTO.ReceiverName,
                ReceiverPhone = createOrderDTO.ReceiverPhone,
                Details = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Xoá sản phẩm khỏi giỏ hàng
            var cart = await _context.Carts.Include(c => c.Details)
                                           .FirstOrDefaultAsync(c => c.CustomerId == createOrderDTO.CustomerId);

            if (cart != null)
            {
                var detailsToRemove = cart.Details?.Where(cd => productDetailIds.Contains(cd.ProductDetailId ?? 0)).ToList();

                if (detailsToRemove?.Any() == true)
                    _context.CartDetails.RemoveRange(detailsToRemove);

                if (cart.Details == null || cart.Details.Count == detailsToRemove?.Count)
                    _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();
            }

            return new OrderResponseDTO
            {
                IsSuccess = true,
                OrderId = order.OrderId,
                Message = "Tạo đơn hàng thành công"
            };
        }
    }
}