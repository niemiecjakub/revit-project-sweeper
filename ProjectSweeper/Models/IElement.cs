using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public interface IElement
    {
        ElementId Id { get; set; }
        string Name { get; set; }
        bool IsUsed { get; set; }
        bool CanBeRemoved { get; set; }
    }
}
