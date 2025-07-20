using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.CustomerAddressRepositories
{
    public interface ICustomerAddressRepository
    {
        Task<List<CustomerAddressViewModel>?> GetAllAsync();
        Task<List<CustomerAddressViewModel>?> GetByCustomerIdAsync(string customerId);
        Task<bool> CreateAsync(CreateUpdateCustomerAddressDTO dto);
        Task<bool> UpdateAsync(CreateUpdateCustomerAddressDTO dto);
        Task <bool> DeleteAsync(int id);
    }

}
