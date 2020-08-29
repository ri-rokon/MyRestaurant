using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyRestaurant.ViewComponenets
{
    public class UserNameViewComponent:ViewComponent
    {

        private readonly ApplicationDbContext _context;

        public UserNameViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == claims.Value);

            return View(userFromDb);
        }

    }
}
