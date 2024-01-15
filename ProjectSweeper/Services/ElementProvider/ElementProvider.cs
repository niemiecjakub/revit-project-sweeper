using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ElementProvider
{
    public class ElementProvider : IElementProvider
    {
        private readonly Document _doc;

        public ElementProvider(Document doc)
        {
            _doc = doc;
        }

        public async Task<IEnumerable<IElement>> GetAllElements(ModelTypes modelType)
        {
            switch (modelType)
            {
                case ModelTypes.LineStyle:
                    return LineFunctions.GetLineStyles(_doc);

                case ModelTypes.LinePattern:
                    return LineFunctions.GetLinePatterns(_doc);

                case ModelTypes.FilledRegion:
                    return FilledRegionFunctions.GetAllFilledRegions(_doc);

                case ModelTypes.FillPattern:
                    return FillPatternFunctions.GetAllFillPatterns(_doc);

                case ModelTypes.Filter:
                    return FilterFunctions.GetAllFilters(_doc);

                case ModelTypes.Viewport:
                    return ViewportFunctions.GetAllViewports(_doc);

                case ModelTypes.ViewTemplate:
                    return ViewFunctions.GetAllViewTemplates(_doc);

                case ModelTypes.Text:
                    return TextFunctions.GetAllText(_doc);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
