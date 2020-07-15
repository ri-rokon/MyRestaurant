﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
       
        [Required]
        public string Name { get; set; }
       
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0, Money=1 }
       
        [Required]
        public double Discount { get; set; }
        
        [Required]
        public double MinimumAmount { get; set; }
        public Byte[]  Picture { get; set; }
        public bool IsActive { get; set; }
    }
}
