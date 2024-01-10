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
                style.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, style); ;
                lineStyles.Add(style);
            }

            IList<Element> curves = GetDocumentCurves(doc);
            SetStyleIsUsed(doc, lineStyles, curves);

            return lineStyles;
        }

        public static IList<Element> GetDocumentCurves(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> curves = collector.WherePasses(new ElementClassFilter(typeof(CurveElement))).ToElements();

            return curves;
        }

        public static void SetStyleIsUsed(Document doc, ISet<LineStyle> allStyles, IList<Element> curves)
        {
            foreach (Element element in curves)
            {
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                {
                    CurveElement curve = element as CurveElement;
                    GraphicsStyle gs = curve.LineStyle as GraphicsStyle;
                    LineStyle styleModel = allStyles.First(ls => ls.Name == gs.Name);
                    styleModel.IsUsed = true;
                }
            }
        }
    }
}
