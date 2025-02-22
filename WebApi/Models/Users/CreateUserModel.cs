using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Users
{
    public class CreateUserModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlySalary { get; set; }
    }
}
