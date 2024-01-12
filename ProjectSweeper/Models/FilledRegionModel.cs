using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class FilledRegionModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.FilledRegion;

        public FilledRegionModel(FilledRegionType filledRegion)
        {
            Id = filledRegion.Id;
            Name = filledRegion.Name;
        }

        public FilledRegionModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
