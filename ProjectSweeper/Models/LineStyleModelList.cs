using Autodesk.Revit.DB;
using ProjectSweeper.Services.ElementRemover;
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
    public class LineStyleModelList
    {
        private readonly ILineStyleProvider _lineStyleProvider;
        private readonly IElementRemover _elementRemover;
        public LineStyleModelList(ILineStyleProvider lineStyleProvider, IElementRemover elementRemover)
        {
            _lineStyleProvider = lineStyleProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<LineStyleModel>> GetAllLineStyles()
        {
            return await _lineStyleProvider.GetAllElements();
        }

        public async Task DeleteLineStyle(IEnumerable<LineStyleModel> lineStyle)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = lineStyle.Select(x => x.Id);
            await _elementRemover.Remove(elementIds);
        }
    }
}
