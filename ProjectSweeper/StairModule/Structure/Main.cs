using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.StairModule.Handrail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Structure
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Create stair elements"))
            {
                transaction.Start();

                IList<Element> floorList = null;
                Reference pickedTunnel = null;
                Reference pickedSlab = null;
                Reference selectedAlignment;
                try
                {
                    //floors
                    ISelectionFilter floorFilter = new FloorFilter();
                    floorList = uidoc.Selection.PickElementsByRectangle(floorFilter, "Select floors");
                    //alignment
                    selectedAlignment = uidoc.Selection.PickObject(ObjectType.Element, "Select alignment");
                    //tunnel
                    pickedTunnel = uidoc.Selection.PickObject(ObjectType.Element, "Select an element");
                    //salab
                    pickedSlab = uidoc.Selection.PickObject(ObjectType.Element, "Select slab");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }


                FloorSelection floorSelection = new FloorSelection(uidoc, doc);
                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();
                string SIDE = stairUI.Side.ToString().ToUpper();

                double BEAM_HEIGHT = Utils.MMToFeetConverter(254);
                double BEAM_WIDTH = Utils.MMToFeetConverter(70);
                double STEP_WIDTH = Utils.MMToFeetConverter(296);
                //double OFFSET_VALUE = SIDE == "R" ? BEAM_WIDTH / 2 : BEAM_WIDTH / 2;
                double OFFSET_VALUE_TO_COLUMN_AX = Utils.MMToFeetConverter(77);

                SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();

                List<Solid> tunnelSolids = Utils.GetSolidFromElement(pickedTunnel, doc);
                List<Solid> slabSolids = Utils.GetSolidFromElement(pickedSlab, doc);
                List<Curve> floorCurves = floorSelection.GetClosesToAlignmentCurveBySelection(floorList, selectedAlignment);

                double maxSpacing = Utils.MMToFeetConverter(3750);

                string IPE_FAMILY_FAME = "HBEAM";
                FamilySymbol horizontalBeamFamily = Utils.GetFamilySymbolByName(IPE_FAMILY_FAME, BuiltInCategory.OST_StructuralFraming, doc);
                HorizontalBeamFactory horizontalBeamFactory = new HorizontalBeamFactory(doc, maxSpacing, horizontalBeamFamily, tunnelSolids);

                string BRACE_FAMILY_NAME = "BRACE";
                FamilySymbol braceFamily = Utils.GetFamilySymbolByName(BRACE_FAMILY_NAME, BuiltInCategory.OST_StructuralFraming, doc);

   
                string SUPPORT_FAMILY_NAME = "SUPPORTCOLUMN";
                FamilySymbol supportFamily = Utils.GetFamilySymbolByName(SUPPORT_FAMILY_NAME, BuiltInCategory.OST_StructuralColumns, doc);
                SupportColumnFactory supportColumnFactory = new SupportColumnFactory(doc, maxSpacing, supportFamily, slabSolids);

                foreach (Curve edgeCurve in floorCurves)
                {
                    //double trimStartValue = Utils.MMToFeetConverter(50);
                    double trimStartValue = SIDE == "R" ? Utils.MMToFeetConverter(296 + 50 + 60) : Utils.MMToFeetConverter(60);
                    double trimEndValue = SIDE == "R" ? Utils.MMToFeetConverter(60) : Utils.MMToFeetConverter(296 + 50 + 60);
                    Curve edgeCurveTrimmed = Utils.TrimStartEndByValue(edgeCurve as Line, trimStartValue, trimEndValue);
                    Line edgeLineOffset = Utils.LineOffset(edgeCurveTrimmed as Line, OFFSET_VALUE_TO_COLUMN_AX);
             
                    //SUPPORT BUILD
                    List<FamilyInstance> supportColumns = supportColumnFactory.BuildAll(SIDE, BEAM_HEIGHT, edgeLineOffset);

                    //BRACE BUILD
                    BraceFactory braceFactory = new BraceFactory(doc, supportColumnFactory.ColumnPlacementlines, braceFamily);
                    List<FamilyInstance> braces = braceFactory.BuildAll();

                    //HBEAM BUILDER 
                    List<FamilyInstance> horizontalBeams = horizontalBeamFactory.BuildAll(SIDE, edgeCurveTrimmed);

                    supportColumnFactory.ColumnPlacementlines.Clear();
                }


                //// L BEAMS BUILDER
                List<Curve> floorLandingSideCurves = floorSelection.GetFloorSideLines(floorList, selectedAlignment);
                string FAMILY_SYMBOL_LANDING_SUPPORT_BEAM = "LBEAM";
                FamilySymbol landingSupportBeamSymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_LANDING_SUPPORT_BEAM, BuiltInCategory.OST_StructuralFraming, doc);
                foreach (Curve landingCurve in floorLandingSideCurves)
                {
                    Line profileLine = Utils.LineOffsetVerically(landingCurve as Line, Utils.MMToFeetConverter(-30));
                    Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
                    FamilyInstance landingSupportBeam = doc.Create.NewFamilyInstance(profileLine, landingSupportBeamSymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                }

                string FAMILY_SYMBOL_EDGE_BEAM = "EDGEBEAM_ADAPTIVE";
                FamilySymbol edgeBeamSymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_EDGE_BEAM, BuiltInCategory.OST_GenericModel, doc);

                List<Line> L_sideBeams = new List<Line>();
                List<Line> R_sideBeams = new List<Line>();

                SideBeamFactory sideBeamFactory = new SideBeamFactory(doc, edgeBeamSymbol);
                foreach (Curve landingCurve in floorLandingSideCurves)
                {
                    double trimValue = Utils.MMToFeetConverter(296 / 2);
                    Line placementLine = Utils.TrimStartEndByValue(landingCurve as Line, STEP_WIDTH / 2, STEP_WIDTH / 2);
                    placementLine = Utils.LineOffset(placementLine, Utils.MMToFeetConverter(-70 / 2));
                    placementLine = Utils.LineOffsetVerically(placementLine, Utils.MMToFeetConverter(-35));

                    int orientationValue = 1;
                    if (placementLine.GetEndPoint(0).X > placementLine.GetEndPoint(1).X)
                    {
                        placementLine = Utils.Reverse(placementLine);
                        orientationValue = 0;
                        L_sideBeams.Add(placementLine);

                    } else
                    {
                        R_sideBeams.Add(placementLine);
                    }
                    FamilyInstance sideBeam = sideBeamFactory.Build(placementLine, orientationValue, "landing");
                }

                L_sideBeams = L_sideBeams.OrderBy(c => c.GetEndPoint(0).X).ToList();
                if (L_sideBeams.Count >= 2)
                { 
                    for (int i = 0; i < L_sideBeams.Count - 1; i++)
                    {
                        int orientationValue = 0;
                        XYZ endPoint = L_sideBeams[i ].GetEndPoint(1);
                        XYZ startPoint = L_sideBeams[i+1].GetEndPoint(0);
                        Line placementLine = Line.CreateBound(startPoint, endPoint);
                        if (placementLine.GetEndPoint(0).X > placementLine.GetEndPoint(1).X)
                        {
                            placementLine = Utils.Reverse(placementLine);

                        }
                        FamilyInstance sideBeam = sideBeamFactory.Build(placementLine,orientationValue, "run");
                    }
                }
                R_sideBeams = R_sideBeams.OrderBy(c => c.GetEndPoint(0).X).ToList();
                if (R_sideBeams.Count >=2 )
                {
                    for (int i = 0; i < R_sideBeams.Count - 1; i++)
                    {
                        int orientationValue = 1;
                        XYZ endPoint = R_sideBeams[i].GetEndPoint(1);
                        XYZ startPoint = R_sideBeams[i+1].GetEndPoint(0);
                        Line placementLine = Line.CreateBound(startPoint, endPoint);
                        if (placementLine.GetEndPoint(0).X > placementLine.GetEndPoint(1).X)
                        {
                            placementLine = Utils.Reverse(placementLine);

                        }
                        FamilyInstance sideBeam = sideBeamFactory.Build(placementLine, orientationValue, "run");
                    }
                }

                transaction.Commit();
            }
                return Result.Succeeded;
        }
    }
}
