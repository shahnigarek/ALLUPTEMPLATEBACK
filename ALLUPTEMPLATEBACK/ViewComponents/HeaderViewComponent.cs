using ALLUPTEMPLATEBACK.ComponentViewModels.Header;
using ALLUPTEMPLATEBACK.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(HeaderVM headerVM)
        {
            return View(headerVM);
        }


    }
}
