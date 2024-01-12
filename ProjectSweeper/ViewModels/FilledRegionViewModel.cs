using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class FilledRegionViewModel : IElementViewModel
    {
        private readonly FilledRegionModel _filledRegion;
        public FilledRegionModel Model => _filledRegion;
        public ElementId Id => _filledRegion.Id;
        public bool IsUsed => _filledRegion.IsUsed;
        public string Name => _filledRegion.Name;
        public bool CanBeRemoved => _filledRegion.CanBeRemoved;

        public FilledRegionViewModel(FilledRegionModel filledRegion)
        {
            _filledRegion = filledRegion;
        }
    }
}
