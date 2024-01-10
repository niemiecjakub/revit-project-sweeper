using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public class LineFunctions
    {
        //LINE STYLES 
        public static ISet<LineStyle> GetLineStyles(Document doc)
        {
            Debug.WriteLine("Line functions");
            ISet<LineStyle> lineStyles = new HashSet<LineStyle>();
            Category c = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            CategoryNameMap subcats = c.SubCategories;
            foreach (Category lineStyle in subcats)
            {
                LineStyle style = new LineStyle(lineStyle);
                lineStyles.Add(style);
            }
            return lineStyles;
        }
    }
}
