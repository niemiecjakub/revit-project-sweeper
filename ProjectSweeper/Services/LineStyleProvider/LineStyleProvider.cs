using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.LineStyleProvider
{
    public class LineStyleProvider : ILineStyleProvider
    {
        //private readonly Document _doc;

        //public LineStyleProvider(Document doc)
        //{
        //    _doc = doc;
        //}

        public IEnumerable<LineStyle> GetAllElements()
        {
            IEnumerable<LineStyle> lineStyles = new List<LineStyle>();
            Debug.WriteLine("Getting all linestyles in provider");
            //IEnumerable<LineStyle> lineStyles = LineFunctions.GetLineStyles(_doc);
            //Debug.WriteLine("Done");

            //foreach (LineStyle lineStyle in lineStyles)
            //{
            //    var name = lineStyle.Name;
            //    Debug.WriteLine($"{name}");
            //}

            return lineStyles;


            //return gameDTOs.Select(g => ToGame(g));
        }
    }
}
