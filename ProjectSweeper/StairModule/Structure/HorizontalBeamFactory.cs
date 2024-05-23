using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjectSweeper.StairModule.Structure
{
    public class HorizontalBeamFactory
    {
        private readonly Document _doc;
        private readonly double _maxSpacing;
        private readonly FamilySymbol _beamFamily;
        private readonly List<Solid> _tunnelSolids;
        private readonly SolidCurveIntersectionOptions _intersectionOptions;
        List<XYZ> BeamXyzList { get; set; } = new List<XYZ>();

        public List<FamilyInstance> Beams { get; set; } = new List<FamilyInstance>();
        public List<Curve> PlacementLines { get; } = new List<Curve>();

        public double PlateRotation { get; set; }

        public HorizontalBeamFactory(Document doc, double maxSpacing, FamilySymbol beamFamily, List<Solid> tunnelSolids) 
        {
            _doc = doc;
            _maxSpacing = maxSpacing;
            _beamFamily = beamFamily;
            _tunnelSolids = tunnelSolids;
            _intersectionOptions = new SolidCurveIntersectionOptions();

        }

        public Line GetPlacementLine(string side, XYZ xyz, Curve placementLine, Line verticalInfinityLine)
        {
            foreach (Solid solid in _tunnelSolids)
            {
                SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, _intersectionOptions);

                    Line perpendicularLineAtPoint = Utils.GetPerpendictularLine(placementLine as Line, xyz, side);
                    XYZ satrtXYZ = Utils.OffsetZValueXYZ(perpendicularLineAtPoint.GetEndPoint(0), Utils.MMToFeetConverter(-254 + 148 / 2));
                    XYZ endXYZ = Utils.OffsetZValueXYZ(perpendicularLineAtPoint.GetEndPoint(1), Utils.MMToFeetConverter(-254 + 148 / 2));
                    perpendicularLineAtPoint = Line.CreateBound(satrtXYZ, endXYZ);
                    SolidCurveIntersection perpendicularLineIntersection = solid.IntersectWithCurve(perpendicularLineAtPoint, _intersectionOptions);
                if (perpendicularLineIntersection.Count() > 0)
                {
                    Curve intersectingCurve = perpendicularLineIntersection.GetCurveSegment(0);
                    XYZ beamEnd = intersectingCurve.GetEndPoint(0);
                    XYZ beamStart = perpendicularLineAtPoint.GetEndPoint(0);
                    Line beamLine = Line.CreateBound(beamStart, beamEnd);
                    PlateRotation = GetPlateRotation(beamLine, solid, 290);
                    return beamLine;

                }
            }
            return Line.CreateBound(new XYZ(), new XYZ(1, 1, 1));
        }

        public List<FamilyInstance> BuildAll(string side, Curve placementLine)
        {
            List<FamilyInstance> builtElements = new List<FamilyInstance>();
            BeamXyzList = Utils.GetXYZlistFromCurve(placementLine, _maxSpacing);
            foreach (XYZ xyz in BeamXyzList)
            {
                Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 2000);
                Line beamLine = GetPlacementLine(side, xyz, placementLine, verticalInfinityLine);
                if (beamLine.GetEndPoint(0).X != 0)
                {
                    try
                    {
                        FamilyInstance beamInstance = Build(beamLine, PlateRotation);
                        Beams.Add(beamInstance);
                        builtElements.Add(beamInstance);
                    } catch (Exception e)
                    {

                    }
                }
            }
            return builtElements;
        }

        public FamilyInstance Build(Line beamPlacementLine, double plateRotation)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;

            FamilyInstance horizontalBeam = _doc.Create.NewFamilyInstance(beamPlacementLine, _beamFamily, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
            Parameter zJustification = horizontalBeam.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
            Parameter startJoinCutBack = horizontalBeam.get_Parameter(BuiltInParameter.START_JOIN_CUTBACK);

            Parameter rightCutAngle = horizontalBeam.LookupParameter("RightCutAngle");
            Parameter leftCutAngle = horizontalBeam.LookupParameter("LeftCutAngle");

            Parameter bhRight = horizontalBeam.LookupParameter("bh_right");
            Parameter bwRight = horizontalBeam.LookupParameter("bw_right");
            Parameter btRight = horizontalBeam.LookupParameter("bt_right");

            Parameter bhLeft = horizontalBeam.LookupParameter("bh_left");
            Parameter bwLeft = horizontalBeam.LookupParameter("bw_left");
            Parameter btLeft = horizontalBeam.LookupParameter("bt_left");

            bhRight.Set(Utils.MMToFeetConverter(580));
            bwRight.Set(Utils.MMToFeetConverter(580));
            btRight.Set(Utils.MMToFeetConverter(16));

            bhLeft.Set(Utils.MMToFeetConverter(148.1));
            bwLeft.Set(Utils.MMToFeetConverter(101.1));
            btLeft.Set(Utils.MMToFeetConverter(10));

            leftCutAngle.Set(90 * (Math.PI / 180.0));
            rightCutAngle.Set((plateRotation * (Math.PI / 180.0)) - Math.PI / 2);

            startJoinCutBack.Set(Utils.MMToFeetConverter(-92.5));
            zJustification.Set(2);

            return horizontalBeam;
        }

        public double GetPlateRotation(Line beamLine, Solid tunnelSolid, double plateOffset)
        {
            try
            {

            List<double> plateOffsets = new List<double>()
                                {
                                    Utils.MMToFeetConverter(plateOffset),
                                    Utils.MMToFeetConverter(-plateOffset)
                                };
            List<XYZ> plateIntersectionPts = new List<XYZ>();
            foreach (double offset in plateOffsets)
            {
                XYZ originXyz = Utils.OffsetZValueXYZ(beamLine.GetEndPoint(1), offset);
                XYZ sp = originXyz + beamLine.Direction * Utils.MMToFeetConverter(2000);
                XYZ ep = originXyz - beamLine.Direction * Utils.MMToFeetConverter(2000);
                Line tempLine = Line.CreateBound(sp, ep);
                SolidCurveIntersection solidIntersectionPlate = tunnelSolid.IntersectWithCurve(tempLine, _intersectionOptions);
                Curve intersectionC = solidIntersectionPlate.GetCurveSegment(0);
                plateIntersectionPts.Add(intersectionC.GetEndPoint(1));
            }
            Line plateSlopeLine = Line.CreateBound(plateIntersectionPts[0], plateIntersectionPts[1]);
            return Utils.GetRotationFromVectors(XYZ.BasisZ, plateSlopeLine.Direction);
            } catch (Exception ex)
            {
                GetPlateRotation(beamLine, tunnelSolid, plateOffset-10);
            }
            return 90 * (180.0 / Math.PI); ;
        }


        public List<FamilyInstance> BuildAllSwitch(string side, Curve placementLine)
        {
            BeamXyzList = GetXYZPoints(placementLine);
            foreach (XYZ xyz in BeamXyzList)
            {
                Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 2000);
                Line beamLine = GetPlacementLine(side, xyz, placementLine, verticalInfinityLine);
                if (beamLine.GetEndPoint(0).X != 0)
                {
                    try
                    {
                        FamilyInstance beamInstance = Build(beamLine, PlateRotation);
                        Beams.Add(beamInstance);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return Beams;
        }

        public List<XYZ> GetXYZPoints(Curve edgeLine)
        {
            List<XYZ> xyzList = new List<XYZ>();

            XYZ startXYZ = edgeLine.GetEndPoint(0);
            XYZ endXYZ = edgeLine.GetEndPoint(1);
            double lineLength = edgeLine.Length;
            int segments = (int)Math.Floor(lineLength / _maxSpacing) + 1;
            lineLength = Utils.FeetToMMConverter(lineLength);

            List<double> divisions = Enumerable.Range(0, segments + 1).Select(i => (double)i / segments).ToList();
            Debug.WriteLine(divisions.Count);
            foreach (double division in divisions)
            {
                Debug.WriteLine(division.ToString());
                XYZ p = edgeLine.Evaluate(division, true);
                xyzList.Add(p);
            }

            Debug.WriteLine($"{lineLength} length");
            Debug.WriteLine($"{segments} SEGMENTS");
            return xyzList;
        }
    }
}
