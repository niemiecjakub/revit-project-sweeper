using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.StairModule.Structure
{
    public class BraceFactory
    {
        private readonly Document _doc;
        private readonly List<Line> _columnPlacementLines;
        private readonly FamilySymbol _braceFamily;
        public List<FamilyInstance> Braces { get; set; } = new List<FamilyInstance>();
        public List<Line> BracePlacementLines { get; set; } = new List<Line>();

        public BraceFactory(Document doc, List<Line> columnPlacementLines, FamilySymbol braceFamily)
        {
            _doc = doc;
            _columnPlacementLines = columnPlacementLines;
            _braceFamily = braceFamily;
        }

        public List<FamilyInstance> BuildAll()
        {
            List<FamilyInstance> builtElements = new List<FamilyInstance>();
            for (int i = 0; i < _columnPlacementLines.Count - 1; i++)
            {
                XYZ basePoint = _columnPlacementLines[i].GetEndPoint(i % 2 == 0 ? 1 : 0);
                XYZ topPoint = _columnPlacementLines[i + 1].GetEndPoint(i % 2 == 0 ? 0 : 1);

                Line placementLine = CreatePlacementLine(basePoint, topPoint);
                FamilyInstance brace =  Build(placementLine);
                builtElements.Add(brace);
            }

            return builtElements;
        }

        public FamilyInstance Build(Line bracePlacementLine)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;

            FamilyInstance brace = _doc.Create.NewFamilyInstance(bracePlacementLine, _braceFamily, datumLevel, StructuralType.Beam);
            Parameter zJustification = brace.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);

            Parameter lVoid = brace.LookupParameter("L_void_l");
            Parameter rVoid = brace.LookupParameter("R_void_l");

            rVoid.Set(Utils.MMToFeetConverter(303.500));
            lVoid.Set(Utils.MMToFeetConverter(303.500));
            zJustification.Set(2);

            Braces.Add(brace);

            return brace;
        }

        private Line CreatePlacementLine(XYZ basePoint, XYZ topPoint)
        {
            double offsetValueToTop = Utils.MMToFeetConverter(-10);
            double offsetValueToBase = Utils.MMToFeetConverter(137 + 20);
            if (topPoint.Z > basePoint.Z)
            {
                topPoint = Utils.OffsetZValueXYZ(topPoint, offsetValueToTop);
                basePoint = Utils.OffsetZValueXYZ(basePoint, offsetValueToBase);
            }
            else
            {
                basePoint = Utils.OffsetZValueXYZ(basePoint, offsetValueToTop);
                topPoint = Utils.OffsetZValueXYZ(topPoint, offsetValueToBase);
            }

            Line bracePlacementLine = Line.CreateBound(basePoint, topPoint);
            BracePlacementLines.Add(bracePlacementLine);
            return bracePlacementLine;
        }
    }
}
