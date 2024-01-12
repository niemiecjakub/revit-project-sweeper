using Autodesk.Revit.DB;
using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Services.FilledRegionProvider;
using ProjectSweeper.Services.LineStyleProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class FilledRegionModelList
    {
        private readonly IFilledRegionProvider _filledRegionProvider;
        private readonly IElementRemover _elementRemover;
        public FilledRegionModelList(IFilledRegionProvider filledRegionProvider, IElementRemover elementRemover)
        {
            _filledRegionProvider = filledRegionProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<IElement>> GetAllFilledRegions()
        {
            return await _filledRegionProvider.GetAllElements();
        }

        public void DeleteFilledRegion(IEnumerable<IElement> filledRegions)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = filledRegions.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }

    }
}
