using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;

namespace MyRestaurant.Areas.Manager.Controllers
{
    [Authorize(Roles = StaticItems.ManagerUser)]

    [Area("Manager")]
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FoodItemsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Manager/FoodItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FoodItem.Include(f => f.Category).Include(f => f.SubCategory).OrderBy(f=>f.CategoryId).OrderBy(f=>f.SubCategoryId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/FoodItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await _context.FoodItem
                .Include(f => f.Category)
                .Include(f => f.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        // GET: Manager/FoodItems/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategory, "Id", "Name");
            return View();
        }

        // POST: Manager/FoodItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image,CategoryId,SubCategoryId,Price")] FoodItem foodItem)
        {
            if (!ModelState.IsValid)
            {
               
                return View(foodItem);
            }
            _context.Add(foodItem);
            await _context.SaveChangesAsync();

            var webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var thisFoodItem = await _context.FoodItem.FindAsync(foodItem.Id);
           // var imageSaveDateTime = DateTime.Now.ToString();


            if (files.Count>0)
            {
                var upload = Path.Combine(webRootPath, "images");
                var extention = Path.GetExtension(files[0].FileName);
                using (var filestream = new FileStream(Path.Combine(upload,thisFoodItem.Id + extention),FileMode.Create))
                {
                    await files[0].CopyToAsync(filestream);
                }

                thisFoodItem.Image = @"\images\"+thisFoodItem.Id + extention ;

            }
            else
            {
                var sourceImage = Path.Combine(webRootPath, "images", StaticItems.defaultImage);
                var destination = Path.Combine(webRootPath, "images", thisFoodItem.Id + ".png");
                System.IO.File.Copy(sourceImage, destination);
                thisFoodItem.Image = @"\images\" + thisFoodItem.Id + ".png";
            }
            await _context.SaveChangesAsync();



           // ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", foodItem.CategoryId);
            //ViewData["SubCategoryId"] = new SelectList(_context.SubCategory, "Id", "Name", foodItem.SubCategoryId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Manager/FoodItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var foodItem = await _context.FoodItem.FindAsync(id);
            //var foodItem = await _context.FoodItem.Include(m => m.Category).Include(m=>m.SubCategory).SingleOrDefaultAsync(m=>m.Id==id);
            if (foodItem == null)
            {
                return NotFound();
            }

            var sub = _context.SubCategory.Where(f => f.CategoryID == foodItem.CategoryId);


            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", foodItem.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(sub, "Id", "Name", foodItem.SubCategoryId);
            return View(foodItem);
        }

        // POST: Manager/FoodItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image,CategoryId,SubCategoryId,Price")] FoodItem foodItem)
        {
            if (id != foodItem.Id)
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {
               
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                
                if (files.Count>0)
                {

                    //NEW IMAGES SHOULD BE SAVE
                    var upload = Path.Combine(webRootPath, "images");
                    var extention = Path.GetExtension(files[0].FileName);
                    var imagePath = Path.Combine(upload, foodItem.Id + extention);
                    var DeleteImagePath = Path.Combine(upload, foodItem.Id + ".png");

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (var filestream = new FileStream(imagePath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(filestream);
                    }

                   foodItem.Image = @"\images\" + foodItem.Id + extention;
                    _context.Update(foodItem);
                }
                else
                {
                    var newImageName = @"\images\" + foodItem.Id + ".png";
                    foodItem.Image = newImageName;
                    _context.Update(foodItem);                    
                    
                }

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }

           // ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", foodItem.CategoryId);
           // ViewData["SubCategoryId"] = new SelectList(_context.SubCategory, "Id", "Name", foodItem.SubCategoryId);
            return View(foodItem);
        }

        // GET: Manager/FoodItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await _context.FoodItem
                .Include(f => f.Category)
                .Include(f => f.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        // POST: Manager/FoodItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodItem = await _context.FoodItem.FindAsync(id);
            _context.FoodItem.Remove(foodItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _context.SubCategory
                                   where subCategory.CategoryID == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Getfristsubcategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _context.SubCategory
                                   where subCategory.CategoryID == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));



        }


        private bool FoodItemExists(int id)
        {
            return _context.FoodItem.Any(e => e.Id == id);
        }
    }
}
