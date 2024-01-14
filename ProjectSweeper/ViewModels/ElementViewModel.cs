using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class ElementViewModel : IElementViewModel
    {
        private readonly IElement _element;
        public IElement Model => _element;
        public ElementId Id => _element.Id;
        public bool IsUsed => _element.IsUsed;
        public string Name => _element.Name;
        public bool CanBeRemoved => _element.CanBeRemoved;

        public ElementViewModel(IElement element)
        {
            _element = element;
        }
    }
}
