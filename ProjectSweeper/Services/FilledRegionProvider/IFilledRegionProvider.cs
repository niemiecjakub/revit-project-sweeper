using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.FilledRegionProvider
{
    public interface IFilledRegionProvider
    {
        Task<IEnumerable<IElement>> GetAllElements();
    }
}
