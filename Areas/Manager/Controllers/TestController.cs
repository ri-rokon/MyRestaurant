using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.ViewModel;

namespace MyRestaurant.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class TestController : Controller
    {
        List<Division> divList = new List<Division>()
        {
            new Division{Id=1,Name="Dhaka"},
            new Division{Id=2,Name="Khulna"}
        };

        

        public IActionResult Create()
        {
            return View(divList);
        }

        public JsonResult GetDistrict(int id)
        {
            if(id<=0)
            {
                return Json(new { result = "failed", mgs = "Id not found" });
            }
            else
            {
                if(id==1)
                {
                    List<string> dhakadistrict = new List<string>()
                    {
                        "Tangail",
                        "Gazipur"
                    };

                    return Json(new { result = "successful ", mgs = "Data found" ,mydata=dhakadistrict});


                }
                else if (id == 2)
                {
                    List<string> khulnadistrict = new List<string>()
                    {
                        "Kustia",
                        "Nator"
                    };
                    return Json(new { result = "successful ", mgs = "data found",mydata=khulnadistrict });

                }
                else
                {
                    return Json(new { result = "failed", mgs = "Id not match" });

                }

            }
        }
    }
   
}
