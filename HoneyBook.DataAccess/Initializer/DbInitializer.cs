using HoneyBook.DataAccess.Data;
using HoneyBook.Models;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext db)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initializer()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_db.Roles.Any(r => r.Name == SD.Role_Admin)) return;

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "AdminTruong",
                    PhoneNumber = "1111111111",
                    StreetAddress = "test 123 Ave",
                    PostalCode = "23422",
                    State = "IL",
                    City = "Chicago"
                }, "Tt123`").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.Where(u => u.Email == "admin@gmail.com").FirstOrDefault();

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            
        }
    }
}
