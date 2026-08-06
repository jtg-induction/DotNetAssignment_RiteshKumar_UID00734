using RestaurantManagement.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [Index("IX_Role_Name", IsUnique = true)]
        public UserRole Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
