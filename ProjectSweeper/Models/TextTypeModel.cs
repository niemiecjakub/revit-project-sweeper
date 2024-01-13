using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class TextTypeModel : IElement
    {
        public ElementId Id {get;set;}
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.Text;

        public TextTypeModel(TextNoteType textNoteType)
        {
            Id = textNoteType.Id;
            Name = textNoteType.Name;
        }

        public TextTypeModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
