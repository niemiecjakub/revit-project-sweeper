using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ViewportProvider
{
    public interface IViewportProvider
    {
        Task<IEnumerable<IElement>> GetAllElements();
    }
}
