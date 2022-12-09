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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);

            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Choosen Category is wrong :(");
                return View(product);
            }
            if (product.BrandId == null)
            {
                ModelState.AddModelError("CategoryId", "Choosen Brand is wrong :(");
                return View(product);
            }
            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Choosen Brand is wrong :( ");
                return View(product);
            }

            List<ProductTag> productTags = new List<ProductTag>();
            foreach (int tagId in product.TagIds)
            {
                if (product.TagIds.Where(t => t == tagId).Count() > 1)
                {
                    ModelState.AddModelError("TagIds", "Tag can be choosen only one time");
                    return View(product);
                }
                if (!await _context.Tags.AnyAsync(c => c.IsDeleted == false && c.Id == tagId)) 
                {
                    ModelState.AddModelError("TagIds", "Choosen Tag is wrong ");
                    return View(product);
                }
                ProductTag productTag = new ProductTag
                {
                    CreatedAt = DateTime.UtcNow.AddHours(+4),
                    CreatedBy = "System",
                    IsDeleted = false,
                    TagId = tagId
                };

                productTags.Add(productTag);
            }

            product.ProductTags = productTags;
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.TagIds = await _context.ProductTags.Where(pt => pt.ProductId == id).Select(x => x.TagId).ToListAsync();

            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Product product, int? id)
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            Product existedproduct = await _context.Products.Include(p => p.ProductTags).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id== id );

            _context.ProductTags.RemoveRange();

            List<ProductTag> productTags = new List<ProductTag>();
            foreach (int tagId in product.TagIds)
            {
                if (product.TagIds.Where(t => t == tagId).Count() > 1)
                {
                    ModelState.AddModelError("TagIds", "Tag can be choosen only one time");
                    return View(product);
                }
                if (!await _context.Tags.AnyAsync(c => c.IsDeleted == false && c.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "Choosen Tag is wrong ");
                    return View(product);
                }
                ProductTag productTag = new ProductTag
                {
                    CreatedAt = DateTime.UtcNow.AddHours(+4),
                    CreatedBy = "System",
                    IsDeleted = false,
                    TagId = tagId
                };

                productTags.Add(productTag);
            }

            existedproduct.ProductTags = productTags;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
