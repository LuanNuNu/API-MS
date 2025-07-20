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
                Address = customerAddress.Address ?? null,
            };
        }
    }
}
