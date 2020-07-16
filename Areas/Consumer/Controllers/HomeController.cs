using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModel;

namespace MyRestaurant.Controllers
{
    [Area("Consumer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ConsumerIndexViewModel IndexViewModel = new ConsumerIndexViewModel()
            {
                FoodItem = await _context.FoodItem.Include(f => f.Category).Include(f => f.SubCategory).ToListAsync(),
                Category = await _context.Category.ToListAsync(),
                Coupon=await _context.Coupon.Where(c=>c.IsActive==true).ToListAsync()



            };
            return View(IndexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
