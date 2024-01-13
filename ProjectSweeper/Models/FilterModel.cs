using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class FilterModel : IElement
    {
        public ElementId Id {get;set;}
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; }

        public FilterModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
        public FilterModel(ParameterFilterElement filterElement)
        {
            Id = filterElement.Id;
            Name = filterElement.Name;
        }
    }
}
