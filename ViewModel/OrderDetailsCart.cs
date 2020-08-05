using MyRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.ViewModel
{
    public class OrderDetailsCart
    {
        public List<CartItem> CartItems { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
