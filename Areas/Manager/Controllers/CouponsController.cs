using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;

namespace MyRestaurant.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Coupons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupon.ToListAsync());
        }

        // GET: Manager/Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // GET: Manager/Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manager/Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CouponType,Discount,MinimumAmount,Picture,IsActive")] Coupon coupon )
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    
                    using ( var fsl = files[0].OpenReadStream())
                    {
                        using (var ms1=new MemoryStream())
                        {
                             fsl.CopyTo(ms1);
                            coupon.Picture = ms1.ToArray();
                        }
                    }

                    _context.Add(coupon);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.msg = "Image Must be uploaded";
                return View(coupon);

            }
            return View(coupon);
        }

        // GET: Manager/Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupon.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Manager/Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CouponType,Discount,MinimumAmount,Picture,IsActive")] Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var thisCoupon = await _context.Coupon.FindAsync(coupon.Id);
                var files = HttpContext.Request.Form.Files;
                if(files.Count>0)
                {
                    using(var fs1= files[0].OpenReadStream() )
                    {
                        using (var ms1=new MemoryStream())
                        {

                            fs1.CopyTo(ms1);
                            thisCoupon.Picture = ms1.ToArray();
                        }
                    }

                }

                thisCoupon.MinimumAmount = coupon.MinimumAmount;
                thisCoupon.Name = coupon.Name;
                thisCoupon.Discount = coupon.Discount;
                thisCoupon.CouponType = coupon.CouponType;
                thisCoupon.IsActive = coupon.IsActive;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Manager/Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Manager/Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupon.FindAsync(id);
            _context.Coupon.Remove(coupon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupon.Any(e => e.Id == id);
        }
    }
}
