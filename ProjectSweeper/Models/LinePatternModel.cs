using Autodesk.Revit.DB;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class LinePatternModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }

        public bool CanBeRemoved {get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.LinePattern;

        public LinePatternModel(string name, ElementId id)
        {
            Name = name;
            Id = id;
        }

        public LinePatternModel(LinePatternElement lpe)
        {
            Name = lpe.Name;
            Id = lpe.Id;
        }

        public LinePatternModel(GraphicsStyle gs)
        {
            ElementId eid = gs.GraphicsStyleCategory.GetLinePatternId(gs.GraphicsStyleType);
            LinePatternElement lpe = gs.Document.GetElement(eid) as LinePatternElement;
            Name = lpe.Name;
            Id = lpe.Id;
        }
    }
}
