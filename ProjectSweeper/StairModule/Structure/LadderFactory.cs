using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Structure
{
    public class LadderFactory
    {
        private readonly Document _doc;
        private readonly double _maxSpacing;
        private readonly FamilySymbol _ladderFamily;
        private readonly List<Solid> _slabSolids;
        public double Rotation { get; set; }
        public List<Line> LadderPlacementlines { get; } = new List<Line>();
        public List<FamilyInstance> Ladders { get; set; } = new List<FamilyInstance>();

        public LadderFactory(Document doc, FamilySymbol ladderFamily, List<Solid> slabSolids)
        {
            _doc = doc;
            _ladderFamily = ladderFamily;
            _slabSolids = slabSolids;
        }

        private double CalculateRotation(Line spanLine)
        {
            XYZ normalVector = Utils.GetNormalVector(spanLine);
            return Utils.GetRotationFromVectors(XYZ.BasisY, normalVector);
        }

        public List<FamilyInstance> BuildAll(string side, double beamHeight, Line spanLine, double zval)
        {
            Rotation = CalculateRotation(spanLine);
            SolidCurveIntersectionOptions intersectionOptions = new SolidCurveIntersectionOptions();

            XYZ endXYZ = spanLine.Evaluate(0.5, true);
            if (Utils.FeetToMMConverter(spanLine.Length) > 3750 ) { 
                if (side == "R")
                {
                    endXYZ = spanLine.Evaluate(0.75, true);
                } else
                {
                    endXYZ = spanLine.Evaluate(0.25, true);
                }
            }


                Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(endXYZ, 500);

                foreach (Solid solid in _slabSolids)
                {
                    SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, intersectionOptions);

                    if (intersection.SegmentCount == 0 || intersection == null)
                    {
                        continue;
                    }

                    Curve intersectingCurve = intersection.GetCurveSegment(0);
                    XYZ basePoint = intersectingCurve.GetEndPoint(0);
                    XYZ endPoint = Utils.OffsetZValueXYZ(endXYZ, -beamHeight);
                    Line columnPlacementline = Line.CreateBound(basePoint, endPoint);

                    FamilyInstance ladder = Build(columnPlacementline, zval);
                    ladder = Rotate(ladder, columnPlacementline, Rotation, side);
                    ladder.LookupParameter("top_offset").Set(Utils.MMToFeetConverter(254));
                    LadderPlacementlines.Add(columnPlacementline);
                    Ladders.Add(ladder);
                }

            return Ladders;
        }

        public FamilyInstance Build(Line placementLine, double zval)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;
            FamilyInstance ladder = _doc.Create.NewFamilyInstance(placementLine.GetEndPoint(0), _ladderFamily, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);

            FilteredElementCollector basePointCollector = new FilteredElementCollector(_doc).OfClass(typeof(BasePoint));

            BasePoint projectBasePoint = basePointCollector.Cast<BasePoint>().FirstOrDefault();
            double projectBasePointElevation = projectBasePoint.Position.Z;

            Parameter baseOffset = ladder.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
            Parameter topOffset = ladder.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

            baseOffset.Set(placementLine.GetEndPoint(0).Z - projectBasePointElevation);
            topOffset.Set(zval - projectBasePointElevation);

            return ladder;
        }

        public FamilyInstance Rotate(FamilyInstance ladder, Line axisLine, double RotationAngleInDegrees, string side)
        {
            RotationAngleInDegrees = side == "R" ? RotationAngleInDegrees : -RotationAngleInDegrees;
            Location locationCurve = ladder.Location;

            if (locationCurve != null)
            {
                XYZ rotationPoint = (axisLine.GetEndPoint(0) + axisLine.GetEndPoint(1)) / 2.0;
                ElementTransformUtils.RotateElement(_doc, ladder.Id, axisLine, RotationAngleInDegrees * (Math.PI / 180.0));
            }

            return ladder;
        }

    }
}
