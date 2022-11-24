using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync(),
                NewArrival = await _context.Products.Where(p => p.IsDeleted == false && p.IsNewArrival).ToListAsync(),
                BestSeller = await _context.Products.Where(p => p.IsDeleted == false && p.IsBestSeller).ToListAsync(),
                Featured = await _context.Products.Where(p => p.IsDeleted == false && p.IsFeatured).ToListAsync()
            };

            return View(homeVM);
        }


        
    }
}
