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
using Stripe;

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
            if(HttpContext.Session.GetString(StaticItems.ssCouponCode)!=null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticItems.ssCouponCode);
                var couponFromDb = await _context.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.TotalOrder = StaticItems.DiscountedPrice(couponFromDb, detailCart.OrderHeader.TotalOrderOriginal);
            }

            
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
        public IActionResult AddCoupon()
        {
            if (detailCart.OrderHeader.CouponCode == null)
            {
                detailCart.OrderHeader.CouponCode = "";
            }
             
            HttpContext.Session.SetString(StaticItems.ssCouponCode, detailCart.OrderHeader.CouponCode);
            return RedirectToAction(nameof(Index));
            
        }
        public IActionResult RemoveCoupon()
        {
            

            HttpContext.Session.SetString(StaticItems.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Summary()
        {

            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.TotalOrder = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _context.ApplicationUser.Where(u => u.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _context.CartItem.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailCart.CartItems = cart.ToList();
            }

            foreach (var list in detailCart.CartItems)
            {
                list.FoodItem = await _context.FoodItem.FirstOrDefaultAsync(m => m.Id == list.FoodItemId);
                detailCart.OrderHeader.TotalOrder = detailCart.OrderHeader.TotalOrder + (list.FoodItem.Price * list.Count);
                
            }
            detailCart.OrderHeader.TotalOrderOriginal = detailCart.OrderHeader.TotalOrder;
            detailCart.OrderHeader.PickupName = applicationUser.Name;
            detailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailCart.OrderHeader.PickUpTime = DateTime.Now;


            if (HttpContext.Session.GetString(StaticItems.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticItems.ssCouponCode);
                var couponFromDb = await _context.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.TotalOrder = StaticItems.DiscountedPrice(couponFromDb, detailCart.OrderHeader.TotalOrderOriginal);
            }


            return View(detailCart);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            detailCart.CartItems = await _context.CartItem.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            detailCart.OrderHeader.PaymentStatus = StaticItems.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.Status = StaticItems.PaymentStatusPending;
            detailCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailCart.OrderHeader.PickUpDate.ToShortDateString() + " " + detailCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _context.OrderHeader.Add(detailCart.OrderHeader);
            await _context.SaveChangesAsync();

            detailCart.OrderHeader.TotalOrderOriginal = 0;


            foreach (var item in detailCart.CartItems)
            {
                item.FoodItem = await _context.FoodItem.FirstOrDefaultAsync(m => m.Id == item.FoodItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    FoodItem = item.FoodItem,
                    OrderId = detailCart.OrderHeader.Id,
                    Description = item.FoodItem.Description,
                    Name = item.FoodItem.Name,
                    Price = item.FoodItem.Price,
                    Count = item.Count
                };
                detailCart.OrderHeader.TotalOrderOriginal += orderDetails.Count * orderDetails.Price;
                _context.OrderDetails.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(StaticItems.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticItems.ssCouponCode);
                var couponFromDb = await _context.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.TotalOrder = StaticItems.DiscountedPrice(couponFromDb, detailCart.OrderHeader.TotalOrderOriginal);
            }
            else
            {
                detailCart.OrderHeader.TotalOrder = detailCart.OrderHeader.TotalOrderOriginal;
            }
            detailCart.OrderHeader.CouponCodeDiscount = detailCart.OrderHeader.TotalOrderOriginal - detailCart.OrderHeader.TotalOrder;

            _context.CartItem.RemoveRange(detailCart.CartItems);
            HttpContext.Session.SetInt32(StaticItems.ssShoppingCartCount, 0);
            await _context.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(detailCart.OrderHeader.TotalOrder * 100),
                Currency = "usd",
                Description = "Order ID : " + detailCart.OrderHeader.Id,
                Source = stripeToken

            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                detailCart.OrderHeader.PaymentStatus = StaticItems.PaymentStatusRejected;
            }
            else
            {
                detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
                detailCart.OrderHeader.PaymentStatus = StaticItems.PaymentStatusApproved;
                detailCart.OrderHeader.Status = StaticItems.StatusSubmitted;
            }
            else
            {
                detailCart.OrderHeader.PaymentStatus = StaticItems.PaymentStatusRejected;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }


    }
}
