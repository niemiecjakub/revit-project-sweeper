using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class ObjectStyleModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.ObjectStyle;

        public ObjectStyleModel(string name, ElementId id)
        {
            Name = name;
            Id = id;
        }

        public ObjectStyleModel(Category category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}
