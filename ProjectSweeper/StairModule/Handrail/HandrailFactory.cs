using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.StairModule.Handrail
{
    public class HandrailFactory
    {
        private readonly Document _doc;
        private readonly FamilySymbol _handrailFamily;
        private readonly List<Curve> _floorEdges;
        public List<FamilyInstance> LandingHandrails { get; set; } = new List<FamilyInstance>();
        public List<FamilyInstance> RunHandrails { get; set; } = new List<FamilyInstance>();
        public List<Line> LandingHandrailPlacementLines { get; set; } = new List<Line>();
        public List<Line> RunHandrailPlacementLines { get; set; } = new List<Line>();


        public HandrailFactory(Document doc, FamilySymbol handrailFamily, List<Curve> floorEdges)
        {   
            _doc = doc;
            _handrailFamily = handrailFamily;
            _floorEdges = floorEdges;
        }

        public Line TranslatePlacementLine(Line placementLine, string side, double handrailHeight, double beamWidth, string purpose)
        {
            double OFFSET_VALUE = side == "R" ? -beamWidth : beamWidth ;
            Line translatedPlacementLine = Utils.LineOffsetVerically(placementLine as Line, handrailHeight);
            if (translatedPlacementLine.Direction.X < 0)
            {
                translatedPlacementLine = Utils.Reverse(translatedPlacementLine);
            }
            if (purpose.Equals("landing"))
            {
                translatedPlacementLine = Utils.TrimStartEndByValue(translatedPlacementLine, 0, Utils.MMToFeetConverter(344));
            }
            translatedPlacementLine = Utils.LineOffset(translatedPlacementLine, OFFSET_VALUE);
            return translatedPlacementLine;
        }

        public FamilyInstance Build(Line placementLine)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;
            FamilyInstance handrail = _doc.Create.NewFamilyInstance(placementLine, _handrailFamily, datumLevel, StructuralType.Beam);
            Parameter zJustification = handrail.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
            zJustification.Set(2);

            return handrail;
        }

        public void BuildLandingHandrails(string side, double handrailHeight, double beamWidth)
        {
            foreach (Curve edgeCurve in _floorEdges)
            {
                Line placementLine = TranslatePlacementLine(edgeCurve as Line, side, handrailHeight, beamWidth, "landing");
                FamilyInstance landingHandrail = Build(placementLine);
                LandingHandrailPlacementLines.Add(placementLine);
                LandingHandrails.Add(landingHandrail);
            }
        }

        public void BuildRunHandrails()
        {
            for (int i = 0; i < LandingHandrailPlacementLines.Count - 1; i++)
            {
                XYZ endPoint = LandingHandrailPlacementLines[i + 1].GetEndPoint(0);
                XYZ startPoint = LandingHandrailPlacementLines[i].GetEndPoint(1);
                Line placementLine = Line.CreateBound(startPoint, endPoint);
                FamilyInstance stairRunHandrail = Build(placementLine);
                RunHandrailPlacementLines.Add(placementLine);
                RunHandrails.Add(stairRunHandrail);
            }
        }
    }
}
