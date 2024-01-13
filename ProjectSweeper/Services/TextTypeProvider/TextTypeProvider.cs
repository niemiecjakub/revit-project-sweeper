using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.TextTypeProvider
{
    public class TextTypeProvider : ITextTypeProvider
    {
        private readonly Document _doc;

        public TextTypeProvider(Document doc)
        {
            _doc = doc;
        }

        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all TEXT in provider");

            IEnumerable<IElement> textList = TextFunctions.GetAllText(_doc);
            Debug.WriteLine($"Total text = {textList.Count()}");
            Debug.WriteLine($"Used text = {textList.Where(t => t.IsUsed).Count()}");
            Debug.WriteLine($"UnUsed text  = {textList.Where(t => !t.IsUsed).Count()}");

            return textList;
        }
    }
}
