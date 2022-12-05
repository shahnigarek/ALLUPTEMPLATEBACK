using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsDeleted == false && c.IsMain)
                .ToListAsync();
            return View(categories);
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain)
                .ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.Categories = await _context.Categories
               .Where(c => c.IsDeleted == false && c.IsMain)
               .ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.Name.ToLower() == category.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", $"This name = {category.Name} already exists ");
                return View(category);
            }

            if (category.IsMain)
            {
                if (category.File == null)
                {
                    ModelState.AddModelError("File", "Fayl Mecburidi");
                    return View(category);
                }
                if (category.File.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Fayl Tipi .jpg ce ya .jpeg olmalidir");
                    return View(category);
                }
                if ((category.File.Length / 1024) > 20)
                {
                    ModelState.AddModelError("File", "Fayl Olcusu maksimum 20 kb olmalidir");
                    return View(category);
                }
                string fileName = Guid.NewGuid().ToString() + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + category.File.FileName;
                string path = @"C:\Users\ROG\source\repos\ALLUPTEMPLATEBACK\ALLUPTEMPLATEBACK\wwwroot\assets\images" + category.File.FileName;
                    using (FileStream fileStream = new FileStream(path, FileMode.Create))
                    {
                    await category.File.CopyToAsync(fileStream);
                    }

                category.ParentId = null;
                category.Image = fileName;
            }
            else
            {
                if (category.ParentId == null)
                {

                    ModelState.AddModelError("ParentId", "Ust Category Mutleq Secilmelidir");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.IsMain && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Duzgun Ust Category Sec");
                    return View(category);
                }
                category.Image = null;
            }

            category.Name = category.Name.Trim();
            category.IsDeleted = false;
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id==null)
            {
                return BadRequest("Id can't be null");
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if(category == null)
            {
                return NotFound("Id is wrong");
            }

            ViewBag.Categories = await _context.Categories
               .Where(c => c.IsDeleted == false && c.IsMain)
               .ToListAsync();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category, int? id)
        {

            ViewBag.Categories = await _context.Categories
               .Where(c => c.IsDeleted == false && c.IsMain)
               .ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (id == null)
            {
                return BadRequest("Id can't be null");
            }

            if (category.Id != id)
            {
                return BadRequest("Id must be equal");
            }
            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.ToLower().Trim() && c.Id != id ))
            {
                ModelState.AddModelError("Name", $"This name = {category.Name} already exists ");
                return View(category);
            }


            Category existedcategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (existedcategory == null)
            {
                return NotFound("Id is wrong");
            }

            if (category.IsMain)
            {
                if ( existedcategory.Image == null  && category.File == null)
                {
                    ModelState.AddModelError("File", "Fayl Mecburidi");
                    return View(category);
                }

                if (category.File != null)
                {
                    if (category.File.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("File", "Fayl Tipi .jpg ce ya .jpeg olmalidir");
                        return View(category);
                    }
                    if ((category.File.Length / 1024) > 20)
                    {
                        ModelState.AddModelError("File", "Fayl Olcusu maksimum 20 kb olmalidir");
                        return View(category);
                    }

                    //string path = @"C:\Users\ROG\source\repos\ALLUPTEMPLATEBACK\ALLUPTEMPLATEBACK\wwwroot\assets\images";

                    string path = Path.Combine(_env.WebRootPath, "assets", "images");

                    if (System.IO.File.Exists(Path.Combine(path,existedcategory.Image)))
                    {
                        System.IO.File.Delete(Path.Combine(path, existedcategory.Image));
                    }

                    string fileName = Guid.NewGuid().ToString() + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + category.File.FileName;
                    
                    string fullpath = Path.Combine(path,fileName);

                    using (FileStream fileStream = new FileStream(fullpath, FileMode.Create))
                    {
                        await category.File.CopyToAsync(fileStream);
                    }

                    existedcategory.ParentId = null;
                    existedcategory.Image = fileName;

                }
            }
            else
            {
                if (category.ParentId == null)
                {

                    ModelState.AddModelError("ParentId", "Ust Category Mutleq Secilmelidir");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.IsMain && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Duzgun Ust Category Sec");
                    return View(category);
                }
                existedcategory.Image = null;
                existedcategory.ParentId = category.ParentId;
            }
            existedcategory.IsMain = category.IsMain;
            existedcategory.Name = category.Name.Trim();
            existedcategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            existedcategory.UpdatedBy= "System";

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id can't be null");
            }

            Category category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null)
            {
                return NotFound("Id is wrong");
            }

            if ((category.Products != null && category.Products.Count() > 0) || (category.Children != null && category.Children.Count() > 0))
            {
                TempData["Error"] = $"Category with {id} Id can't be deleted"; 

                return RedirectToAction("Index");
            }

            category.IsDeleted = true;
            category.DeletedBy = "";
            category.DeletedAt= DateTime.UtcNow.AddHours(4);

            //_context.Categories.Remove(category);-birdefelik silmek ucun istifade olunur bunu istifade edende artiq  bunlara  ( category.IsDeleted = true;
            //category.DeletedBy = "";
            //category.DeletedAt = DateTime.UtcNow.AddHours(4); ) ehtiyyac qalmir

            //RemoveRange var o collection seklinde silir
            //AddRangeAsync siyahi seklinde elave edir database
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
     
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id can't be null");
            }

            Category category = await _context.Categories
                .Include(c=>c.Products)
                .Include(c=> c.Children)
                .FirstOrDefaultAsync(c=> c.IsDeleted == false && c.IsMain && c.Id == id);

            if (category == null)
            {
                return NotFound("Id is wrong");
            }

            return View(category);
        }
    }
}
