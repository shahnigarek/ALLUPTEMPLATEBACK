using ALLUPTEMPLATEBACK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Interfaces
{
     public interface ILayoutServices
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
    }
}
