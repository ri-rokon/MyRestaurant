using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
      //  private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        //ILogger<HomeController> logger,
        public HomeController( ApplicationDbContext context)
        {
           // _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ConsumerIndexViewModel IndexViewModel = new ConsumerIndexViewModel()
            {
                FoodItem = await _context.FoodItem.Include(f => f.Category).Include(f => f.SubCategory).ToListAsync(),
                Category = await _context.Category.ToListAsync(),
                Coupon = await _context.Coupon.Where(c => c.IsActive == true).ToListAsync()



            };
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim !=null)
            {
                var cnt = _context.CartItem.Where(c => c.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32("ssCount", cnt);
            }
             return View(IndexViewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var FoodItemDb = await _context.FoodItem.Include(f => f.Category).Include(f => f.SubCategory).FirstOrDefaultAsync(f => f.Id == id);
            
            CartItem cartItem=new CartItem()
            {
                FoodItem = FoodItemDb,
                FoodItemId = FoodItemDb.Id

            };
            return View(cartItem);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(CartItem cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                CartItem cartItemFromDb = await _context.CartItem.FirstOrDefaultAsync(c => c.ApplicationUserId == cartObj.ApplicationUserId && c.FoodItemId==cartObj.FoodItemId);
                if (cartItemFromDb == null)
                {
                    await _context.CartItem.AddAsync(cartObj);
                }
                else
                {
                    cartItemFromDb.Count = cartItemFromDb.Count + cartObj.Count;
                }
                await _context.SaveChangesAsync();
                var count= _context.CartItem.Where(c=>c.ApplicationUserId==cartObj.ApplicationUserId).Count();
                HttpContext.Session.SetInt32("ssCount", count);
                return RedirectToAction("Index")  ;



            }
            else
            {
                var FoodItemDb = await _context.FoodItem.Include(f => f.Category).Include(f => f.SubCategory).FirstOrDefaultAsync(f => f.Id == cartObj.Id);

                CartItem cartItem = new CartItem()
                {
                    FoodItem = FoodItemDb,
                    FoodItemId = FoodItemDb.Id

                };
                return View(cartItem);

            }



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
