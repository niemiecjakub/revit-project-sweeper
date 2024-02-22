using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.StairModule.Structure
{
    public class SupportColumnFactory
    {
        private readonly Document _doc;
        private readonly double _maxSpacing;
        private readonly FamilySymbol _supportFamily;
        private readonly List<Solid> _slabSolids;
        public double Rotation { get; set; }
        public List<Line> ColumnPlacementlines { get; } = new List<Line>();
        public List<XYZ> PlacementPoints { get; set; } = new List<XYZ>();
        public List<FamilyInstance> Supports { get; set; } = new List<FamilyInstance>();

        public SupportColumnFactory(Document doc, double maxSpacing, FamilySymbol supportFamily, List<Solid> slabSolids)
        {
            _doc = doc;
            _maxSpacing = maxSpacing;
            _supportFamily = supportFamily;
            _slabSolids = slabSolids;
        }

        private double CalculateRotation(Line spanLine)
        {
            XYZ normalVector = Utils.GetNormalVector(spanLine);
            return Utils.GetRotationFromVectors(XYZ.BasisY, normalVector);
        }

        public List<FamilyInstance> BuildAll(string side, double beamHeight, Line spanLine)
        {
            Rotation = CalculateRotation(spanLine);
            PlacementPoints = Utils.GetXYZlistFromCurve(spanLine, _maxSpacing);
            SolidCurveIntersectionOptions intersectionOptions = new SolidCurveIntersectionOptions();
            foreach (XYZ xyz in PlacementPoints)
            {
                Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 500);

                foreach (Solid solid in _slabSolids)
                {
                    SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, intersectionOptions);

                    if (intersection.SegmentCount == 0 || intersection == null)
                    {
                        continue;
                    }

                    Curve intersectingCurve = intersection.GetCurveSegment(0);
                    XYZ basePoint = intersectingCurve.GetEndPoint(0);
                    XYZ endPoint = Utils.OffsetZValueXYZ(xyz, -beamHeight);
                    Line columnPlacementline = Line.CreateBound(basePoint, endPoint);

                    FamilyInstance supportColumn = Build(columnPlacementline);
                    supportColumn = Rotate(supportColumn, columnPlacementline, Rotation, side);
                    ColumnPlacementlines.Add(columnPlacementline);
                    Supports.Add(supportColumn);
                }
            }

            return Supports;
        }

        public FamilyInstance Build(Line placementLine)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;
            FamilyInstance supportColumn = _doc.Create.NewFamilyInstance(placementLine.GetEndPoint(0), _supportFamily, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);

            FilteredElementCollector basePointCollector = new FilteredElementCollector(_doc).OfClass(typeof(BasePoint));

            BasePoint projectBasePoint = basePointCollector.Cast<BasePoint>().FirstOrDefault();
            double projectBasePointElevation = projectBasePoint.Position.Z;
            //Debug.WriteLine("PBP: " + Utils.FeetToMMConverter(projectBasePointElevation));

            Parameter baseOffset = supportColumn.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
            Parameter topOffset = supportColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

            baseOffset.Set(placementLine.GetEndPoint(0).Z - projectBasePointElevation);
            topOffset.Set(placementLine.GetEndPoint(1).Z - projectBasePointElevation);

            return supportColumn;
        }

        public FamilyInstance Rotate(FamilyInstance supportColumn, Line axisLine, double RotationAngleInDegrees, string side)
        {
            RotationAngleInDegrees = side == "R" ? RotationAngleInDegrees : -RotationAngleInDegrees;
            Location locationCurve = supportColumn.Location;

            if (locationCurve != null)
            {
                XYZ rotationPoint = (axisLine.GetEndPoint(0) + axisLine.GetEndPoint(1)) / 2.0;
                ElementTransformUtils.RotateElement(_doc, supportColumn.Id, axisLine, RotationAngleInDegrees * (Math.PI / 180.0));
            }

            return supportColumn;
        }

        private double CalculateRotationRamp(Line spanLine)
        {
            return Utils.GetRotationFromVectors(XYZ.BasisY, XYZ.BasisZ);
        }

        public List<FamilyInstance> BuildAllRamp(string side, double beamHeight, Line spanLine)
        {
            Rotation = CalculateRotationRamp(spanLine);
            PlacementPoints = Utils.GetXYZlistFromCurve(spanLine, _maxSpacing);
            SolidCurveIntersectionOptions intersectionOptions = new SolidCurveIntersectionOptions();
            foreach (XYZ xyz in PlacementPoints)
            {
                Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 500);

                foreach (Solid solid in _slabSolids)
                {
                    SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, intersectionOptions);

                    if (intersection.SegmentCount == 0 || intersection == null)
                    {
                        continue;
                    }

                    Curve intersectingCurve = intersection.GetCurveSegment(0);
                    XYZ basePoint = intersectingCurve.GetEndPoint(0);
                    XYZ endPoint = Utils.OffsetZValueXYZ(xyz, -beamHeight);
                    Line columnPlacementline = Line.CreateBound(basePoint, endPoint);

                    FamilyInstance supportColumn = Build(columnPlacementline);
                    supportColumn = Rotate(supportColumn, columnPlacementline, Rotation, side);
                    ColumnPlacementlines.Add(columnPlacementline);
                    Supports.Add(supportColumn);
                }
            }

            return Supports;
        }
    }
}
