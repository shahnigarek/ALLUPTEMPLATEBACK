using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Search(int? id,string search)
        {
            IEnumerable<ProductListVM> products = await _context.Products
                .Where(
                p => id != null ? p.CategoryId == id : true &&
                p.Title.ToLower().Contains(search.ToLower()) ||
                p.Brand.Name.ToLower().Contains(search.ToLower()))
                .OrderByDescending(p=>p.Id)
                .Take(3)
                .Select(x => new ProductListVM
                {
                    Id = x.Id,
                    Title=x.Title,
                    Image=x.MainImage
                }).ToListAsync();
                
                
               return PartialView("_SearchPartial",products) ;
        }

        public async Task<IActionResult> Modal (int? id)
        {
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
            return PartialView("_ProductModalPartial",product);
        }
    }
}
