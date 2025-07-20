using Microsoft.AspNetCore.Identity;

namespace MenShop_Assignment.Datas
{
    public class User : IdentityUser
    {
        public bool? Gender { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DisabledDate { get; set; }
        public bool? IsDisabled { get; set; }
        public int? BranchId { get; set; }
        public Branch? WorkedBranch { get; set; }
        public Branch? ManagedBranch { get; set; }
        public string? EmployeeAddress { get; set; }
        public string? ManagerId { get; set; }
        public User? Manager { get; set; }
        public Cart? CustomerCart { get; set; } 
        public ICollection<User>? Employees { get; set; }
        public ICollection<OutputReceipt>? OutputReceipts { get; set; }
        public ICollection<InputReceipt>? InputReceipts { get; set; }
        public ICollection<Storage>? Storages { get; set; }
        public ICollection<Order>? CustomerOrders { get; set; }
        public ICollection<Order>? EmployeesOrders { get; set; }
        public ICollection<Order>? ShipperOrders { get; set; }
        public ICollection<CustomerAddress>? CustomerAddresses { get; set; }
    }
}
