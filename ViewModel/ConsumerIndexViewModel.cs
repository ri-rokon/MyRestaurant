using MyRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.ViewModel
{
    public class ConsumerIndexViewModel
    {
        public IEnumerable<FoodItem> FoodItem { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }
    }
}
