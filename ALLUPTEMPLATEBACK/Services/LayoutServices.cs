using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Interfaces;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.ViewModels.Basket;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Services
{
    public class LayoutServices: ILayoutServices
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutServices(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBasketAsync()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                basketVM.Title = product.Title;
                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.Image = product.MainImage;
                basketVM.ExTax = product.ExTax;
            }

                return basketVMs;
        }

        public async  Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Include(c => c.Children).Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
        }

        public async  Task<Dictionary<string,string>> GetSettingsAsync()
        {
            return  await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
        }

    }
}