using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.ViewModels.Home;
using Microsoft.AspNetCore.Http;
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
            //HomeVM homeVM = new HomeVM
            //{
            //    //Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),
            //    //Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync(),
            //    NewArrival = await _context.Products.Where(p => p.IsDeleted == false && p.IsNewArrival).ToListAsync(),
            //    BestSeller = await _context.Products.Where(p => p.IsDeleted == false && p.IsBestSeller).ToListAsync(),
            //    Featured = await _context.Products.Where(p => p.IsDeleted == false && p.IsFeatured).ToListAsync()
            //};

            return View(/*homeVM*/);
        }
        //public async Task<IActionResult> Setcookie()
        //{
        //    HttpContext.Response.Cookies.Append("P228", "MyFirstCookie");

        //    return RedirectToAction(nameof(Index));
        //}
        //public async Task<IActionResult> Getcookie()
        //{
        //    return Content(HttpContext.Request.Cookies["P228"]);
        //}

        //public async Task<IActionResult> SetSession()
        //{
        //    HttpContext.Session.SetString("P228", "My First Session");
        //    return RedirectToAction(nameof(Index));
        //}
        //public async Task<IActionResult> GetSession()
        //{
        //    return Content(HttpContext.Session.GetString("My First Session"));
        //}

    }
}
