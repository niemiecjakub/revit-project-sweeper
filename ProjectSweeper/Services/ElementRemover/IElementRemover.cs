using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ElementRemover
{
    public interface IElementRemover
    {
        Task Remove(IEnumerable<ElementId> eIds);
    }
}
