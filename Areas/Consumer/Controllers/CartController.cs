using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using MyRestaurant.ViewModel;

namespace MyRestaurant.Areas.Consumer.Controllers
{
    [Area("Consumer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.TotalOrder = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _context.CartItem.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailCart.CartItems = cart.ToList();
            }

            foreach (var list in detailCart.CartItems)
            {
                list.FoodItem = await _context.FoodItem.FirstOrDefaultAsync(m => m.Id == list.FoodItemId);
                detailCart.OrderHeader.TotalOrder = detailCart.OrderHeader.TotalOrder + (list.FoodItem.Price * list.Count);
                list.FoodItem.Description = list.FoodItem.Description;
                if (list.FoodItem.Description.Length > 100)
                {
                    list.FoodItem.Description = list.FoodItem.Description.Substring(0, 99) + "...";
                }
            }
            detailCart.OrderHeader.TotalOrderOriginal = detailCart.OrderHeader.TotalOrder;
            return View(detailCart);

        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _context.CartItem.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _context.CartItem.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _context.CartItem.Remove(cart);
                await _context.SaveChangesAsync();

                var cnt = _context.CartItem.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;

                HttpContext.Session.SetInt32("ssCount", cnt);
            }
            else
            {
                cart.Count -= 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _context.CartItem.FirstOrDefaultAsync(c => c.Id == cartId);

            _context.CartItem.Remove(cart);
            await _context.SaveChangesAsync();

            var cnt = _context.CartItem.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("ssCount", cnt);
            return RedirectToAction(nameof(Index));
        }

    }
}
