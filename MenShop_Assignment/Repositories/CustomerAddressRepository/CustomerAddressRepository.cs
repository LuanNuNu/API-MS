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
            var customerAddresses = await _context.CustomerAddresses
                .Include(ca => ca.Customer)
                .ToListAsync();

            return customerAddresses
                .Select(CustomerAddressMapper.ToCustomerAddressViewModel)
                .ToList();
        }

        public async Task<List<CustomerAddressViewModel>?> GetByCustomerIdAsync(string customerId)
        {
            var customerAddresses = await _context.CustomerAddresses
                .Include(ca => ca.Customer)
                .Where(ca => ca.CustomerId == customerId)
                .ToListAsync();

            return customerAddresses
                .Select(CustomerAddressMapper.ToCustomerAddressViewModel)
                .ToList();
        }

        public async Task<bool> CreateAsync(CreateUpdateCustomerAddressDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.CustomerId);
            if (user == null) return false;

            var address = new CustomerAddress
            {
                CustomerId = dto.CustomerId,
                ProvinceId = dto.ProvinceId,
                ProvinceName = dto.ProvinceName,
                DistrictId = dto.DistrictId,
                DistrictName = dto.DistrictName,
                WardId = dto.WardId,
                WardName = dto.WardName,
                Street = dto.Street,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone
            };

            await _context.CustomerAddresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, CreateUpdateCustomerAddressDTO dto)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            address.ProvinceId = dto.ProvinceId;
            address.ProvinceName = dto.ProvinceName;
            address.DistrictId = dto.DistrictId;
            address.DistrictName = dto.DistrictName;
            address.WardId = dto.WardId;
            address.WardName = dto.WardName;
            address.Street = dto.Street;
            address.ReceiverName = dto.ReceiverName;
            address.ReceiverPhone = dto.ReceiverPhone;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }


}
