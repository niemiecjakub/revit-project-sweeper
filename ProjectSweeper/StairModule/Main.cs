using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Debug.WriteLine("STARTING");
            using (Transaction transaction = new Transaction(doc, "Create Lines"))
            {
                transaction.Start();

                List<Reference> pickedEdges = null;
                Reference pickedTunnel = null;
                Reference pickedSlab = null;
                try
                {
                    Selection selection = uidoc.Selection;
                    pickedEdges = selection.PickObjects(ObjectType.Edge, "Select edges").ToList();
                    pickedTunnel = selection.PickObject(ObjectType.Element, "Select an element");
                    pickedSlab = selection.PickObject(ObjectType.Element, "Select an element");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }
                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();
                string SIDE = stairUI.Side.ToString().ToUpper();

                double BEAM_HEIGHT = Utils.MMToFeetConverter(240);
                double BEAM_WIDTH = Utils.MMToFeetConverter(85);
                double STEP_WIDTH = Utils.MMToFeetConverter(296);
                double OFFSET_VALUE = SIDE == "R" ? BEAM_WIDTH / 2 : BEAM_WIDTH / 2;

                SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();

                List<Solid> tunnelSolids = Utils.GetSolidFromElement(pickedTunnel, doc);
                List<Solid> slabSolids = Utils.GetSolidFromElement(pickedSlab, doc);

                foreach (Reference pickedEdge in pickedEdges)
                {
                    Curve edgeLine = Utils.GetCurveFromEdge(pickedEdge, doc);
                    Curve edgeLineTrimmed = Utils.TrimStartEndByValue(edgeLine as Line, Utils.MMToFeetConverter(50), Utils.MMToFeetConverter(296 + 50));
                    Line edgeLineOffset = Utils.LineOffset(edgeLineTrimmed as Line, OFFSET_VALUE);

                    XYZ normalVector = Utils.GetNormalVector(edgeLineOffset);
                    double supportColumnRotation = Utils.GetRotationFromVectors(XYZ.BasisX, normalVector);

                    List<Line> columnPlacementlines = new List<Line>();
                    List<XYZ> xyzList = Utils.GetXYZlistFromCurve(edgeLineOffset, Utils.MMToFeetConverter(3750));
                    foreach (XYZ xyz in xyzList)
                    {
                        string FAMILY_SYMBOL_NAME_COLUMN = "RK50X5";
                        FamilySymbol columnFamilySymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_NAME_COLUMN, BuiltInCategory.OST_StructuralColumns, doc);
                        Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 500);


                        foreach (Solid solid in slabSolids)
                        {
                            SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, intersectOptions);

                            if (intersection.SegmentCount == 0 || intersection == null)
                            {
                                continue;
                            }

                            Curve intersectingCurve = intersection.GetCurveSegment(0);
                            XYZ intersectingCurveStartXYZ = intersectingCurve.GetEndPoint(0);
                            XYZ endPoint = Utils.OffsetZValueXYZ(xyz, -BEAM_HEIGHT);
                            Line columnPlacementline = Line.CreateBound(intersectingCurveStartXYZ, endPoint);

                            FamilyInstance columnInstance = SupportColumnBuilder.CreateColumnByIntersection(doc, columnPlacementline, columnFamilySymbol);
                            SupportColumnBuilder.Rotate(columnInstance, columnPlacementline, supportColumnRotation, SIDE);
                            columnPlacementlines.Add(columnPlacementline);

                        }
                    }

                    string FAMILY_SYMBOL_NAME_BRACE = "BRACE_RK50X5";
                    FamilySymbol braceFamilySymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_NAME_BRACE, BuiltInCategory.OST_StructuralColumns, doc);
                    for (int i = 0; i < columnPlacementlines.Count - 1; i++)
                    {
                        XYZ basePoint = columnPlacementlines[i].GetEndPoint(i % 2 == 0 ? 1 : 0);
                        XYZ topPoint = columnPlacementlines[i + 1].GetEndPoint(i % 2 == 0 ? 0 : 1);

                        Line bracePlacementLine = Line.CreateBound(basePoint, topPoint);
                        SketchPlane braceSketchPlane = Utils.CreateSketchPlaneByCurve(bracePlacementLine, doc);
                        doc.Create.NewModelCurve(bracePlacementLine, braceSketchPlane);
                        
                        if (i % 2 == 0)
                        {
                            SlantedColumnBuilder.CreateSlantedColumn(doc, topPoint, basePoint, braceFamilySymbol);
                        }
                        else
                        {
                            SlantedColumnBuilder.CreateSlantedColumn(doc, basePoint, topPoint, braceFamilySymbol);
                        }
                    }


                    List<XYZ> beamXyzList = Utils.GetXYZlistFromCurve(edgeLineTrimmed, Utils.MMToFeetConverter(3750));
                    string FAMILY_SYMBOL_NAME_BEAM = "IPE140";
                    FamilySymbol horizontalBeamFamilySymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_NAME_BEAM, BuiltInCategory.OST_StructuralFraming, doc);
                    foreach (XYZ xyz in beamXyzList)
                    {
                        Line verticalInfinityLine = Utils.CreateVerticalCurveFromXYZ(xyz, 500);
                        foreach (Solid solid in tunnelSolids)
                        {
                            SolidCurveIntersection intersection = solid.IntersectWithCurve(verticalInfinityLine, intersectOptions);

                            if (intersection.SegmentCount == 0 || intersection == null)
                            {
                                continue;
                            }

                            try
                            {

                                Line perpendicularLineAtPoint = Utils.GetPerpendictularLine(edgeLineTrimmed as Line, xyz, SIDE);

                                XYZ satrtXYZ = Utils.OffsetZValueXYZ(perpendicularLineAtPoint.GetEndPoint(0), Utils.MMToFeetConverter(-240 + 70));
                                XYZ endXYZ = Utils.OffsetZValueXYZ(perpendicularLineAtPoint.GetEndPoint(1), Utils.MMToFeetConverter(-240 + 70));
                                perpendicularLineAtPoint = Line.CreateBound(satrtXYZ, endXYZ);

                                SolidCurveIntersection perpendicularLineIntersection = solid.IntersectWithCurve(perpendicularLineAtPoint, intersectOptions);
                                Curve intersectingCurve = perpendicularLineIntersection.GetCurveSegment(0);

                                XYZ beamEnd = intersectingCurve.GetEndPoint(0);
                                XYZ beamStart = perpendicularLineAtPoint.GetEndPoint(0);

                                Line beamLine = Line.CreateBound(beamStart, beamEnd);
                                SketchPlane perpendicularLineSketchPlane = Utils.CreateSketchPlaneByCurve(beamLine, doc);
                                Plane plane = Utils.CreatePlaneByCurve(beamLine, doc);

                                List<double> plateOffsets = new List<double>()
                                {
                                    Utils.MMToFeetConverter(290),
                                    Utils.MMToFeetConverter(-290)
                                };
                                List<XYZ> plateIntersectionPts = new List<XYZ>();
                                foreach (double offset in plateOffsets)
                                {
                                    XYZ originXyz = Utils.OffsetZValueXYZ(beamEnd, offset);
                                    XYZ sp = originXyz + beamLine.Direction * Utils.MMToFeetConverter(1000);
                                    XYZ ep = originXyz - beamLine.Direction * Utils.MMToFeetConverter(1000);
                                    Line tempLine = Line.CreateBound(sp, ep);
                                    SolidCurveIntersection solidIntersectionPlate = solid.IntersectWithCurve(tempLine, intersectOptions);
                                    Curve intersectionC = solidIntersectionPlate.GetCurveSegment(0);
                                    plateIntersectionPts.Add(intersectionC.GetEndPoint(1));
                                }

                                Line plateSlopeLine = Line.CreateBound(plateIntersectionPts[0], plateIntersectionPts[1]);
                                double tunnelPlateRotation = Utils.GetRotationFromVectors(XYZ.BasisZ, plateSlopeLine.Direction);
                                FamilyInstance horizontalBeam = HorizontalBeamBuilder.CreateHorizontalBeam(doc, horizontalBeamFamilySymbol, beamLine, tunnelPlateRotation);
                            } catch (Exception ex)
                            {

                            }
                        }
                    }

                }
                transaction.Commit();

            }
            return Result.Succeeded;
        }

    }
}
