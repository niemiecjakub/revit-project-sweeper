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

        public async Task<IEnumerable<LinePatternModel>> GetAllLinePatterns()
        {
            return await _linePatternProvider.GetAllElements();
        }

        public async Task DeleteLinePatetrn(IEnumerable<LinePatternModel> linePatterns)
        {
            Debug.WriteLine($"LINE PATTERN LIST: inside line pattern list"); ;
            IEnumerable<ElementId> elementIds = linePatterns.Select(x => x.Id);
            await _elementRemover.Remove(elementIds);
        }
    }
}
