using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Controllers
{
    public class ModalController : Controller
    {
        private readonly AppDbContext _context;
        public ModalController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
     
        public async Task<IActionResult> Modal(int? id)
        {
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            return PartialView("_ModalPartial", product);
        }
    }
}
