using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class FillPatternModel : IElement
    {
        public ElementId Id {get;set;}
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.FillPattern;

        public FillPatternModel(FillPatternElement fillPattern)
        {
            Id = fillPattern.Id;
            Name = fillPattern.Name;
        }

        public FillPatternModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
