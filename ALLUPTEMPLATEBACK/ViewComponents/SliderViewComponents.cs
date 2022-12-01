using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Models;
using ALLUPTEMPLATEBACK.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.ViewComponents
{
    public class SliderViewComponents : ViewComponent
    {
        private readonly AppDbContext _context;

        public SliderViewComponents(AppDbContext context )
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {


            IEnumerable<Slider> sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync();
            return View(await Task.FromResult(sliders));
        }

    }
}
