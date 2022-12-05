using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Where(p => p.IsDeleted == false)
                .ToListAsync();

            return View(products);
        }
    }
}
