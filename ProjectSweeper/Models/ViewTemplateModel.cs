using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class ViewTemplateModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.ViewTemplate;

        public ViewTemplateModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
        public ViewTemplateModel(View viewTemplate)
        {
            Id = viewTemplate.Id;
            Name = viewTemplate.Name;
        }
    }
}
