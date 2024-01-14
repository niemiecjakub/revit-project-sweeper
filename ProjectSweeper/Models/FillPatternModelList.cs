using Autodesk.Revit.DB;
using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Services.FilledRegionProvider;
using ProjectSweeper.Services.FillPatternProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class FillPatternModelList : IModelList
    {
        private readonly IFillPatternProvider _fillPatternProvider;
        private readonly IElementRemover _elementRemover;
        public FillPatternModelList(IFillPatternProvider fillPatternProvider, IElementRemover elementRemover)
        {
            _fillPatternProvider = fillPatternProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<IElement>> GetAllFillPatterns()
        {
            return await _fillPatternProvider.GetAllElements();
        }

        public void DeleteFillPattern(IEnumerable<IElement> fillPatterns)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = fillPatterns.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }

        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            return await _fillPatternProvider.GetAllElements();
        }

        public void DeleteElements(IEnumerable<IElement> elements)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = elements.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }
    }
}
