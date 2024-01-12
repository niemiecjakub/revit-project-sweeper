using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public interface IElementManager
    {
        bool CanRemoveElements { get; }
    }
}
