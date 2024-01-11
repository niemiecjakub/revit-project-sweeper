using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class LinePatternViewModel : ViewModelBase
    {
        private readonly LinePatternModel _linePattern;
        public ElementId Id => _linePattern.Id;
        public bool IsUsed => _linePattern.IsUsed;
        public string Name => _linePattern.Name;
        public bool CanBeRemoved => _linePattern.CanBeRemoved;

        public LinePatternViewModel(LinePatternModel linePattern)
        {
            _linePattern = linePattern;
        }
    }
}
