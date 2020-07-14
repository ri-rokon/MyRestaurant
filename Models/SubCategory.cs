using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "SubCategory Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

    }
}
