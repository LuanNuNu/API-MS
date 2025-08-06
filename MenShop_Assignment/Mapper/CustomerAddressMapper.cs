using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class CustomerAddressMapper
    {
        public static CustomerAddressViewModel ToCustomerAddressViewModel(CustomerAddress customerAddress)
        {
            return new CustomerAddressViewModel
            {
                Id = customerAddress.Id,
                CustomerName = customerAddress.Customer?.FullName ?? null,
                ReceiverPhone = customerAddress.ReceiverPhone ?? null,
                ReceiverName = customerAddress.ReceiverName ?? null,
                Street = customerAddress.Street ?? null,
                DistrictId = customerAddress.DistrictId ?? null,
                DistrictName = customerAddress.DistrictName ?? null,
                ProvinceId = customerAddress.ProvinceId ?? null,
                ProvinceName = customerAddress.ProvinceName ?? null,
                WardId = customerAddress.WardId ?? null,
                WardName = customerAddress.WardName ?? null
            };
        }
    }
}
