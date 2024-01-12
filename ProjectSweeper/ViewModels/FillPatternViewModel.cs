using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class FillPatternViewModel : IElementViewModel
    {
        private readonly FillPatternModel _fillPattern;
        public FillPatternModel Model => _fillPattern;
        public ElementId Id => _fillPattern.Id;
        public bool IsUsed => _fillPattern.IsUsed;
        public string Name => _fillPattern.Name;
        public bool CanBeRemoved => _fillPattern.CanBeRemoved;

        public FillPatternViewModel(FillPatternModel fillPattern)
        {
            _fillPattern = fillPattern;
        }
    }
}
