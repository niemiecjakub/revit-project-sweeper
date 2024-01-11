using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class LineStyleViewModel : ViewModelBase
    {
        private readonly LineStyle _lineStyle;
        public ElementId Id => _lineStyle.Id;
        public bool IsUsed => _lineStyle.IsUsed;
        public string Name => _lineStyle.Name;
        public bool CanBeRemoved => _lineStyle.CanBeRemoved;

        public LineStyleViewModel(LineStyle lineStyle)
        {
            _lineStyle = lineStyle;
        }
    }
}
