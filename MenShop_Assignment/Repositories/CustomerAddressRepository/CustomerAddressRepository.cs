using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.DTOs;
namespace MenShop_Assignment.Repositories.CustomerAddressRepositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerAddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerAddressViewModel>?> GetAllAsync()
        {
            var customerAddresses = await _context.CustomerAddresses.ToListAsync();
            return customerAddresses.Select(CustomerAddressMapper.ToCustomerAddressViewModel).ToList() ?? [];
        }


        public async Task<List<CustomerAddressViewModel>?> GetByCustomerIdAsync(string customerId)
        {
            var customerAddresses = await _context.CustomerAddresses
                .Where(ca => ca.CustomerId == customerId)
                .ToListAsync();

			return customerAddresses.Select(CustomerAddressMapper.ToCustomerAddressViewModel).ToList() ?? [];
		}

        public async Task<bool> CreateAsync(CreateUpdateCustomerAddressDTO dto)
        {
            var user = await _context.Users.Where(x=>x.Id== dto.CustomerId).FirstOrDefaultAsync();
            if (user == null)
                return false;

            await _context.CustomerAddresses.AddAsync(new CustomerAddress { CustomerId = dto.CustomerId, Address = dto.Address });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(CreateUpdateCustomerAddressDTO dto)
        {
            var address = await _context.CustomerAddresses.FirstOrDefaultAsync(cd => cd.Id == dto.Id);
            if (address == null || dto.Id==0) 
                return false;

            address.Address = dto.Address;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var address = await _context.CustomerAddresses.FirstOrDefaultAsync(cd => cd.Id == id);
            if (address == null)
                return false;

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
