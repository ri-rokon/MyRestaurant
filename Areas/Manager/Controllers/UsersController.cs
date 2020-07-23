using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;

namespace MyRestaurant.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController( ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _context.ApplicationUser.Where(u=>u.Id != claim.Value).ToListAsync());
        }
    }
}