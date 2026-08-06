using RestaurantManagement.Enums;
using RestaurantManagement.Models;
using System.Data.Entity.Migrations;

namespace RestaurantManagement.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<RestaurantManagement.Data.RestaurantDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RestaurantManagement.Data.RestaurantDbContext context)
        {
            context.Roles.AddOrUpdate(
                role => role.RoleId,

                new Role
                {
                    RoleId = (int)UserRole.User,
                    Name = UserRole.User
                },

                new Role
                {
                    RoleId = (int)UserRole.SuperAdmin,
                    Name = UserRole.SuperAdmin
                }
            );
        }
    }
}
