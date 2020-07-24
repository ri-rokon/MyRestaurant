using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Models;

namespace MyRestaurant.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MyRestaurant.Models.Category> Category { get; set; }
        public DbSet<MyRestaurant.Models.SubCategory> SubCategory { get; set; }
        public DbSet<MyRestaurant.Models.FoodItem> FoodItem { get; set; }
        public DbSet<MyRestaurant.Models.Coupon> Coupon { get; set; }
        public DbSet<MyRestaurant.Models.ApplicationUser> ApplicationUser { get; set; }
        public DbSet<MyRestaurant.Models.Cart> Cart { get; set; }

    }
}
