using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSweeper.RevitFunctions
{
    public class LineFunctions
    {
        //LINE STYLES 
        public static ISet<LineStyleModel> GetLineStyles(Document doc)
        {
            Debug.WriteLine("Line functions");
            ISet<LineStyleModel> lineStyles = new HashSet<LineStyleModel>();
            Category c = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            CategoryNameMap subcats = c.SubCategories;
            foreach (Category lineStyle in subcats)
            {
                LineStyleModel style = new LineStyleModel(lineStyle);
                style.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, lineStyle.Id); ;
                lineStyles.Add(style);
            }

            IList<Element> curves = GetDocumentCurves(doc);
            SetStyleIsUsed(lineStyles, curves);

            return lineStyles;
        }

        public static IList<Element> GetDocumentCurves(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> curves = collector.WherePasses(new ElementClassFilter(typeof(CurveElement))).ToElements();

            return curves;
        }

        public static void SetStyleIsUsed(ISet<LineStyleModel> allStyles, IList<Element> curves)
        {
            foreach (Element element in curves)
            {
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                {
                    CurveElement curve = element as CurveElement;
                    GraphicsStyle gs = curve.LineStyle as GraphicsStyle;
                    LineStyleModel styleModel = allStyles.First(ls => ls.Name == gs.Name);
                    styleModel.IsUsed = true;
                }
            }
        }


        //LINE PATTERNS 
        public static ISet<LinePatternModel> GetLinePatterns(Document doc)
        {
            ISet<LinePatternModel> linePatterns = new HashSet<LinePatternModel>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ISet<Element> linePatternsFound = collector.OfClass(typeof(LinePatternElement)).ToHashSet();

            foreach (Element linePattern in linePatternsFound)
            {
                if (linePattern is LinePatternElement lpe)
                {
                    LinePatternModel lpm = new LinePatternModel(lpe);
                    lpm.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, linePattern.Id); ;
                    linePatterns.Add(lpm);
                }
            }
            LinePatternModel solid = new LinePatternModel("Solid", LinePatternElement.GetSolidPatternId());
            linePatterns.Add(solid);

            IList<Element> curves = GetDocumentCurves(doc);
            SetPatternIsUsed(linePatterns, curves);

            return linePatterns;
        }

        public static void SetPatternIsUsed(ISet<LinePatternModel> allPatterns, IList<Element> curves)
        {

            foreach (Element element in curves)
            {
                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                {
                    CurveElement curve = element as CurveElement;
                    GraphicsStyle gs = curve.LineStyle as GraphicsStyle;

                    ElementId eid = gs.GraphicsStyleCategory.GetLinePatternId(gs.GraphicsStyleType);
                    LinePatternElement pattern = gs.Document.GetElement(eid) as LinePatternElement;

                    LinePatternModel patternModel = allPatterns.First(p => p.Id == eid);
                    patternModel.IsUsed = true;
                }
            }
        }
    }
}
