using ALLUPTEMPLATEBACK.DAL;
using ALLUPTEMPLATEBACK.Interfaces;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Services
{
    public class LayoutServices: ILayoutServices
    {
        private readonly AppDbContext _context;
        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }
        public async  Task<Dictionary<string,string>> GetSettingsAsync()
        {
            return  await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
        }

    }
}