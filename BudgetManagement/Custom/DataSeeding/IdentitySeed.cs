using ExpenseManagment.Data.DataBaseEntities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseManagment.Custom.DataSeeding
{
    public class IdentitySeed
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync(Helper.SuperAdminUserName.ToString()).Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = Helper.SuperAdminUserName.ToString();
                user.Email = Helper.SuperAdminUserName.ToString();
                user.EmailConfirmed = true;
                IdentityResult result = userManager.CreateAsync
                (user, "SuperAdmin@123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Helper.PageRoles.SuperAdmin.ToString()).Wait();
                }
            }

            if (userManager.FindByNameAsync(Helper.AdminUserName.ToString()).Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = Helper.AdminUserName.ToString();
                user.Email = Helper.AdminUserName.ToString();
                user.EmailConfirmed = true;
                IdentityResult result = userManager.CreateAsync
                (user, "Admin@123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Helper.PageRoles.Admin.ToString()).Wait();
                }
            }
        }

        static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (string RoleName in Enum.GetNames(typeof(Helper.PageRoles)))
            {
                if (!roleManager.RoleExistsAsync(RoleName).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = RoleName;
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
                }
            }
        }
    }
}
