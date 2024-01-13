using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ViewportProvider
{
    public class ViewportProvider : IViewportProvider
    {
        private readonly Document _doc;

        public ViewportProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all VIEWPORTS in provider");
            IEnumerable<IElement> viewports = ViewportFunctions.GetAllViewports(_doc);
            return viewports;
        }
    }
}
