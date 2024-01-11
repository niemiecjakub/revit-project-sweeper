using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.LinePatternProvider
{
    public interface ILinePatternProvider
    {
        Task<IEnumerable<LinePatternModel>> GetAllElements();
    }
}
