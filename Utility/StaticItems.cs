using MyRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Utility
{
    public static class StaticItems
    {
        public const string defaultImage = "default_food.png";
        public const string ManagerUser = "Manager";
        public const string FrontDeskUser = "FrontDesk";
        public const string ConsumerUser = "Consumer";
        public const string CookerUser = "Cooker";


        public const string ssCouponCode = "ssCouponCode";
        public const string ssShoppingCartCount = "ssCartCount";


        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";



        public static double DiscountedPrice(Coupon couponFromDb, double originalTotal)
        {
            if(couponFromDb ==null)
            {
                return originalTotal;

            }
            else
            {
                if(couponFromDb.MinimumAmount>originalTotal)
                {
                    return originalTotal;
                }
                else
                {
                    //Every Things is ok
                    if(Convert.ToInt32(couponFromDb.CouponType)==(int)Coupon.ECouponType.Money)
                    {
                        //$10 off $100
                        return Math.Round(originalTotal - couponFromDb.Discount, 2);
                    }
                    if(Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
                    {
                        //10% off $100
                        return Math.Round(originalTotal - (originalTotal * couponFromDb.Discount / 100), 2);
                    }
                }

            }
            return originalTotal;

        }

    }
}
