using Autodesk.Revit.DB;
using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Services.LinePatternProvider;
using ProjectSweeper.Services.LineStyleProvider;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class LinePatternModelList 
    {
        private readonly ILinePatternProvider _linePatternProvider;
        private readonly IElementRemover _elementRemover;
        public LinePatternModelList(ILinePatternProvider linePatternProvider, IElementRemover elementRemover)
        {
            _linePatternProvider = linePatternProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<IElement>> GetAllLinePatterns()
        {
            return await _linePatternProvider.GetAllElements();
        }

        public void DeleteLinePatetrn(IEnumerable<IElement> linePatterns)
        {
            Debug.WriteLine($"LINE PATTERN LIST: inside line pattern list"); ;
            IEnumerable<ElementId> elementIds = linePatterns.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }
    }
}
