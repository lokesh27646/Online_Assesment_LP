using System.Collections.Generic;
using BusinessEntities;

namespace WebApi.Models.Users
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public UserTypes Type { get; set; }
        public decimal? MonthlySalary { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}