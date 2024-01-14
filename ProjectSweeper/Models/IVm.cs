using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public interface IVm
    {
        IElement Element { get; }
        ElementId Id { get; }
        bool IsUsed { get; }
        string Name { get; }
        bool CanBeRemoved { get; }
    }
}
