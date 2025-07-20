using System.ComponentModel.DataAnnotations;

namespace MenShop_Assignment.Models.Account
{
    public class AccountRegister
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
		public string? EmployeeAddress { get; set; }
        public int? BranchId { get; set; }
		public string? Role { get; set; }
	}
}
