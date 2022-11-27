using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALLUPTEMPLATEBACK.ViewModels.Basket;

namespace ALLUPTEMPLATEBACK.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id can't be null");
            }

            //Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id))
            {
                return NotFound("Id is wrong");
            }

            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> products = null;

            if (!string.IsNullOrWhiteSpace(basket))
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                BasketVM basketVM = products.Find(p => p.Id == id);
                if (basketVM != null)
                {
                    basketVM.Count += 1;
                }
                else
                {
                    basketVM = new BasketVM 
                    { 
                      Id= (int)id,
                      Count= 1
                    };

                    products.Add(basketVM);
                }
            }
            else
            {
                 products = new List<BasketVM>();
               BasketVM basketVM = new BasketVM
                {
                    Id = (int)id,
                    Count = 1
                };

                products.Add(basketVM);
            }
          
            basket = JsonConvert.SerializeObject(products);
            HttpContext.Response.Cookies.Append("basket", basket); 

            return RedirectToAction("Index","Home");


        }

        public async Task<IActionResult> GetFromBasket(int? id)
        {
            string pro = HttpContext.Request.Cookies["basket"];
            List<BasketVM> products = JsonConvert.DeserializeObject<List<BasketVM>>(pro);
            return Json(products);
        }
    }
}
