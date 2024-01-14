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
    public class LineStyleModelList : IModelList
    {
        private readonly ILineStyleProvider _lineStyleProvider;
        private readonly IElementRemover _elementRemover;
        public LineStyleModelList(ILineStyleProvider lineStyleProvider, IElementRemover elementRemover)
        {
            _lineStyleProvider = lineStyleProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<IElement>> GetAllLineStyles()
        {
            return await _lineStyleProvider.GetAllElements();
        }

        public void DeleteLineStyle(IEnumerable<IElement> lineStyle)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = lineStyle.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }

        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            return await _lineStyleProvider.GetAllElements();
        }

        public void DeleteElements(IEnumerable<IElement> elements)
        {
            Debug.WriteLine($"LINE STYLES LIST: inside line styles list"); ;
            IEnumerable<ElementId> elementIds = elements.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }
    }
}
