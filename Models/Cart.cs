using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Models
{
    public class Cart
    {
        public Cart()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int FoodItemId { get; set; }
        [NotMapped]
        [ForeignKey("FoodItemId")]
        public virtual FoodItem FoodItem { get; set; }

        [Range(1,int.MaxValue, ErrorMessage ="Plese Enter a value greater then or equal to {1}")]
        public int Count { get; set; }
    }
}
