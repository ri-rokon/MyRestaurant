using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Data
{

    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context,
                             UserManager<IdentityUser> userManager,
                             RoleManager<IdentityRole> roleManager )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }


        public async void Initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count()>0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_context.Roles.Any(r => r.Name == StaticItems.ManagerUser)) return;


            _roleManager.CreateAsync(new IdentityRole(StaticItems.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticItems.ConsumerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticItems.FrontDeskUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticItems.ManagerUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Ridwanul Islam",
                EmailConfirmed = true,
                PhoneNumber = "01680046122"

            }, "AAaa!!11").GetAwaiter().GetResult();

            IdentityUser user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
            await _userManager.AddToRoleAsync(user, StaticItems.ManagerUser);
                
        }
    }
}
