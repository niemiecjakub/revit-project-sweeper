using Autodesk.Revit.DB;
using ProjectSweeper.Services.ElementProvider;
using ProjectSweeper.Services.ElementRemover;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class ElementModelList : IModelList
    {
        private readonly IElementProvider _elementProvider;
        private readonly IElementRemover _elementRemover;
        public ElementModelList(IElementProvider filledRegionProvider, IElementRemover elementRemover)
        {
            _elementProvider = filledRegionProvider;
            _elementRemover = elementRemover;
        }

        public async Task<IEnumerable<IElement>> GetAllElements(ModelTypes modelType)
        {
            return await _elementProvider.GetAllElements(modelType);
        }

        public void DeleteElements(IEnumerable<IElement> elements)
        {
            Debug.WriteLine($"MODEL LIST"); ;
            IEnumerable<ElementId> elementIds = elements.Select(x => x.Id);
            _elementRemover.Remove(elementIds);
        }
    }
}
